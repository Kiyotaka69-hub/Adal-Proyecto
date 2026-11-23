using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoneyTrack.Application;
using MoneyTrack.Domain;
namespace MoneyTrack.Infrastructure;

public sealed class FinanzasRepository(MoneyTrackContext db):IFinanzasRepository
{
    public Task<List<Categoria>> Categorias(int u,CancellationToken ct) => db.Categorias.AsNoTracking()
        .Where(x=>x.IdUsuario==u).OrderBy(x=>x.Nombre).ThenBy(x=>x.IdCategoria).ToListAsync(ct);
    public Task<Categoria?> Categoria(int u,int id,CancellationToken ct) =>
        db.Categorias.SingleOrDefaultAsync(x=>x.IdUsuario==u && x.IdCategoria==id,ct);
    public Task<Movimiento?> Movimiento(int u,int id,CancellationToken ct) =>
        db.Movimientos.SingleOrDefaultAsync(x=>x.IdUsuario==u && x.IdMovimiento==id,ct);
    public async Task<PaginaMovimientos> Historial(int u,FiltroMovimientos f,CancellationToken ct)
    {
        var query=db.Movimientos.AsNoTracking().Where(x=>x.IdUsuario==u);
        if(f.Desde.HasValue) query=query.Where(x=>x.Fecha>=f.Desde);
        if(f.Hasta.HasValue) query=query.Where(x=>x.Fecha<=f.Hasta);
        if(f.IdCategoria.HasValue) query=query.Where(x=>x.IdCategoria==f.IdCategoria);
        if(f.Tipo!=null) query=query.Where(x=>x.Tipo==f.Tipo);
        var total=await query.CountAsync(ct);
        var orden=f.Orden=="asc" ? query.OrderBy(x=>x.Fecha).ThenBy(x=>x.IdMovimiento) :
            query.OrderByDescending(x=>x.Fecha).ThenByDescending(x=>x.IdMovimiento);
        return new(await orden.Skip((f.Pagina-1)*f.Tamano).Take(f.Tamano).ToListAsync(ct),total,f.Pagina,f.Tamano);
    }
    public void Agregar<T>(T entidad) where T:class => db.Add(entidad);
    public void Eliminar<T>(T entidad) where T:class => db.Remove(entidad);
    public async Task Guardar(CancellationToken ct)
    {
        try { await db.SaveChangesAsync(ct); }
        catch(DbUpdateConcurrencyException) { throw new FinanzasException(409,"El registro cambió durante la operación."); }
        catch(DbUpdateException e) when(e.InnerException is SqlException { Number: 547 or 2601 or 2627 })
        { throw new FinanzasException(409,"La operación entra en conflicto con un registro existente."); }
    }
}
