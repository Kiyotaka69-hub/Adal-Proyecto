using System.ComponentModel.DataAnnotations;
using MoneyTrack.Domain;
namespace MoneyTrack.Application;

public record DatosPresupuesto(int IdCategoria, decimal Limite, [Range(1,12)] int Mes, [Range(1,9999)] int Anio);
public record GastoCategoria(int IdCategoria,string Nombre,string Tipo,decimal Total);
public record Avance(int IdPresupuesto,int IdCategoria,string Nombre,decimal Limite,int Mes,int Anio,string Origen,decimal Gastado);
public interface IResumenRepository
{
    Task<List<GastoCategoria>> Estadisticas(int u,int mes,int anio,CancellationToken ct);
    Task<decimal> Saldo(int u,CancellationToken ct);
    Task<List<Avance>> Avances(int u,int mes,int anio,CancellationToken ct);
    Task<Presupuesto?> Buscar(int u,int id,CancellationToken ct);
}
public sealed class PresupuestosService(IResumenRepository resumen,IFinanzasRepository finanzas)
{
    public async Task<Presupuesto> Guardar(int u,int? id,DatosPresupuesto datos,CancellationToken ct)
    {
        if(datos.Limite<=0 || datos.Limite>9999999999.99m || decimal.Round(datos.Limite,2)!=datos.Limite)
            throw new FinanzasException(400,"Importe inválido.");
        var categoria=await finanzas.Categoria(u,datos.IdCategoria,ct) ?? throw new FinanzasException(404,"Categoría no encontrada.");
        if(categoria.Tipo!="Gasto") throw new FinanzasException(400,"Selecciona una categoría de gasto.");
        var presupuesto=id.HasValue ? await Buscar(u,id.Value,ct) : new Presupuesto { IdUsuario=u,Origen="Manual" };
        presupuesto.IdCategoria=datos.IdCategoria;
        presupuesto.Limite=datos.Limite;
        presupuesto.Mes=datos.Mes;
        presupuesto.Anio=datos.Anio;
        if(!id.HasValue) finanzas.Agregar(presupuesto);
        await finanzas.Guardar(ct);
        return presupuesto;
    }
    private async Task<Presupuesto> Buscar(int u,int id,CancellationToken ct) =>
        await resumen.Buscar(u,id,ct) ?? throw new FinanzasException(404,"Presupuesto no encontrado.");
    public async Task Eliminar(int u,int id,CancellationToken ct)
    {
        finanzas.Eliminar(await Buscar(u,id,ct));
        await finanzas.Guardar(ct);
    }
    public Task<List<Avance>> Avances(int u,int m,int a,CancellationToken ct) => resumen.Avances(u,m,a,ct);
    public Task<List<GastoCategoria>> Estadisticas(int u,int m,int a,CancellationToken ct) => resumen.Estadisticas(u,m,a,ct);
    public async Task<object> Resumen(int u,int m,int a,CancellationToken ct)
    {
        var stats=await Estadisticas(u,m,a,ct);
        return new { mes=m,anio=a,saldo=await resumen.Saldo(u,ct),ingresos=stats.Where(x=>x.Tipo=="Ingreso").Sum(x=>x.Total),
            gastos=stats.Where(x=>x.Tipo=="Gasto").Sum(x=>x.Total) };
    }
}
