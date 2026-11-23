using Microsoft.EntityFrameworkCore;
using MoneyTrack.Application;
using MoneyTrack.Domain;
namespace MoneyTrack.Infrastructure;

public sealed class PrediccionRepository(MoneyTrackContext db):IPrediccionRepository
{
    public Task<List<MesCategoria>> Totales(int u,DateOnly desde,DateOnly hasta,CancellationToken ct) =>
        db.Movimientos.Where(x=>x.IdUsuario==u && x.Tipo=="Gasto" && x.Fecha>=desde && x.Fecha<hasta)
        .GroupBy(x=>new { x.IdCategoria,x.Fecha.Year,x.Fecha.Month })
        .Select(g=>new MesCategoria(g.Key.IdCategoria,g.Key.Year,g.Key.Month,g.Sum(x=>x.Cantidad))).ToListAsync(ct);
    public Task<List<CategoriaGasto>> Categorias(int u,CancellationToken ct) =>
        db.Categorias.Where(x=>x.IdUsuario==u && x.Tipo=="Gasto").OrderBy(x=>x.IdCategoria)
        .Select(x=>new CategoriaGasto(x.IdCategoria,x.Nombre)).ToListAsync(ct);
    public async Task Guardar(int u,DateOnly periodo,IReadOnlyList<Pronostico> filas,CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        foreach (var fila in filas)
        {
            if(fila.Estimacion is not Estimacion e)
            {
                await db.Predicciones.Where(x=>x.IdUsuario==u && x.IdCategoria==fila.IdCategoria && x.Anio==periodo.Year && x.Mes==periodo.Month).ExecuteDeleteAsync(ct);
                continue;
            }
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE Predicciones WITH (UPDLOCK,SERIALIZABLE)
                SET monto_predicho={e.Monto}, confianza={e.Confianza}, metodo={PrediccionService.Metodo}, fecha_generada=SYSUTCDATETIME()
                WHERE id_usuario={u} AND id_categoria={fila.IdCategoria} AND mes={periodo.Month} AND anio={periodo.Year};
                IF @@ROWCOUNT=0
                INSERT Predicciones(id_usuario,id_categoria,mes,anio,monto_predicho,confianza,metodo,fecha_generada)
                VALUES({u},{fila.IdCategoria},{periodo.Month},{periodo.Year},{e.Monto},{e.Confianza},{PrediccionService.Metodo},SYSUTCDATETIME());
                """,ct);
        }
        await tx.CommitAsync(ct);
    }
    public async Task Conversar(int u,string pregunta,string respuesta,CancellationToken ct)
    {
        var fecha=DateTime.UtcNow;
        db.ConversacionesIA.AddRange(new MensajeIA{IdUsuario=u,Rol="Usuario",Contenido=pregunta,Fecha=fecha},
            new MensajeIA{IdUsuario=u,Rol="Asistente",Contenido=respuesta,Fecha=fecha.AddTicks(1)});
        await db.SaveChangesAsync(ct);
    }
    public async Task<List<LineaChat>> Historial(int u,CancellationToken ct)
    {
        var ultimos=await db.ConversacionesIA.Where(x=>x.IdUsuario==u).OrderByDescending(x=>x.Fecha).ThenByDescending(x=>x.IdMensaje)
            .Take(50).Select(x=>new LineaChat(x.Rol,x.Contenido,x.Fecha)).ToListAsync(ct);
        ultimos.Reverse();
        return ultimos;
    }
}
