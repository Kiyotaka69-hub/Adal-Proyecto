using System.ComponentModel.DataAnnotations;
using MoneyTrack.Domain;
namespace MoneyTrack.Application;

public record DatosCategoria([Required, MaxLength(100)] string Nombre, [Required, RegularExpression("Ingreso|Gasto")] string Tipo);
public record DatosMovimiento(int IdCategoria, decimal Cantidad, [Required, RegularExpression("Ingreso|Gasto")] string Tipo,
    [MaxLength(500)] string? Descripcion, DateOnly Fecha, bool EsRecurrente);
public sealed class FiltroMovimientos
{
    public DateOnly? Desde { get; set; }
    public DateOnly? Hasta { get; set; }
    public int? IdCategoria { get; set; }
    [RegularExpression("Ingreso|Gasto")] public string? Tipo { get; set; }
    [Range(1,1000000)] public int Pagina { get; set; } = 1;
    [Range(1,100)] public int Tamano { get; set; } = 25;
    [RegularExpression("asc|desc")] public string Orden { get; set; } = "desc";
}
public record PaginaMovimientos(List<Movimiento> Items, int Total, int Pagina, int Tamano);
public interface IFinanzasRepository
{
    Task<List<Categoria>> Categorias(int usuario, CancellationToken ct);
    Task<Categoria?> Categoria(int usuario, int id, CancellationToken ct);
    Task<Movimiento?> Movimiento(int usuario, int id, CancellationToken ct);
    Task<PaginaMovimientos> Historial(int usuario, FiltroMovimientos filtro, CancellationToken ct);
    void Agregar<T>(T entidad) where T:class;
    void Eliminar<T>(T entidad) where T:class;
    Task Guardar(CancellationToken ct);
}
public sealed class FinanzasException(int status, string mensaje) : Exception(mensaje)
{
    public int Status { get; } = status;
}
public sealed class FinanzasService(IFinanzasRepository repo)
{
    public Task<List<Categoria>> Categorias(int usuario, CancellationToken ct) => repo.Categorias(usuario,ct);
    private async Task<Categoria> Categoria(int u,int id,CancellationToken ct) =>
        await repo.Categoria(u,id,ct) ?? throw new FinanzasException(404,"Categoría no encontrada.");
    public async Task<Categoria> GuardarCategoria(int u,int? id,DatosCategoria datos,CancellationToken ct)
    {
        var categoria = id.HasValue ? await Categoria(u,id.Value,ct) : new Categoria { IdUsuario=u, Nombre="", Tipo=datos.Tipo };
        if(categoria.Tipo != datos.Tipo) throw new FinanzasException(409,"El tipo de una categoría creada es fijo.");
        categoria.Nombre=datos.Nombre.Trim();
        if(!id.HasValue) repo.Agregar(categoria);
        await repo.Guardar(ct);
        return categoria;
    }
    public async Task EliminarCategoria(int u,int id,CancellationToken ct)
    {
        repo.Eliminar(await Categoria(u,id,ct));
        await repo.Guardar(ct);
    }
    public Task<PaginaMovimientos> Historial(int u,FiltroMovimientos filtro,CancellationToken ct)
    {
        if(filtro.Desde > filtro.Hasta) throw new FinanzasException(400,"Intervalo de fechas inválido.");
        return repo.Historial(u,filtro,ct);
    }
    public async Task<Movimiento> Movimiento(int u,int id,CancellationToken ct) =>
        await repo.Movimiento(u,id,ct) ?? throw new FinanzasException(404,"Movimiento no encontrado.");
    public async Task<Movimiento> GuardarMovimiento(int u,int? id,DatosMovimiento datos,CancellationToken ct)
    {
        if(datos.Cantidad <= 0 || datos.Cantidad > 9999999999.99m || decimal.Round(datos.Cantidad,2)!=datos.Cantidad || datos.Fecha==default)
            throw new FinanzasException(400,"Revisa el importe y la fecha.");
        var categoria=await Categoria(u,datos.IdCategoria,ct);
        if(categoria.Tipo!=datos.Tipo) throw new FinanzasException(400,"La categoría y el movimiento deben tener el mismo tipo.");
        var mov=id.HasValue ? await Movimiento(u,id.Value,ct) : new Movimiento { IdUsuario=u, Tipo=datos.Tipo };
        mov.IdCategoria=categoria.IdCategoria;
        mov.Tipo=datos.Tipo;
        mov.Fecha=datos.Fecha;
        mov.Cantidad=datos.Cantidad;
        mov.Descripcion=datos.Descripcion?.Trim();
        mov.EsRecurrente=datos.EsRecurrente;
        if(!id.HasValue) repo.Agregar(mov);
        await repo.Guardar(ct);
        return mov;
    }
    public async Task EliminarMovimiento(int u,int id,CancellationToken ct)
    {
        repo.Eliminar(await Movimiento(u,id,ct));
        await repo.Guardar(ct);
    }
}
