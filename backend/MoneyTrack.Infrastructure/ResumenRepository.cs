using Microsoft.EntityFrameworkCore;
using MoneyTrack.Application;
using MoneyTrack.Domain;
namespace MoneyTrack.Infrastructure;

public sealed class ResumenRepository(MoneyTrackContext db):IResumenRepository
{
    public Task<List<GastoCategoria>> Estadisticas(int u,int m,int a,CancellationToken ct) =>
        (from x in db.Movimientos join c in db.Categorias on x.IdCategoria equals c.IdCategoria
         where x.IdUsuario==u && x.Fecha.Month==m && x.Fecha.Year==a
         group x by new { c.IdCategoria,c.Nombre,c.Tipo } into g
         orderby g.Key.Nombre,g.Key.IdCategoria
         select new GastoCategoria(g.Key.IdCategoria,g.Key.Nombre,g.Key.Tipo,g.Sum(x=>x.Cantidad))).ToListAsync(ct);
    public Task<decimal> Saldo(int u,CancellationToken ct) => db.Movimientos.Where(x=>x.IdUsuario==u)
        .SumAsync(x=>x.Tipo=="Ingreso" ? x.Cantidad : -x.Cantidad,ct);
    public Task<List<Avance>> Avances(int u,int m,int a,CancellationToken ct) =>
        (from p in db.Presupuestos join c in db.Categorias on p.IdCategoria equals c.IdCategoria
         where p.IdUsuario==u && p.Mes==m && p.Anio==a orderby c.Nombre,p.IdPresupuesto
         select new Avance(p.IdPresupuesto,p.IdCategoria,c.Nombre,p.Limite,m,a,p.Origen,
             db.Movimientos.Where(x=>x.IdUsuario==u && x.IdCategoria==p.IdCategoria && x.Fecha.Month==m && x.Fecha.Year==a && x.Tipo=="Gasto")
             .Sum(x=>(decimal?)x.Cantidad) ?? 0)).ToListAsync(ct);
    public Task<Presupuesto?> Buscar(int u,int id,CancellationToken ct) =>
        db.Presupuestos.SingleOrDefaultAsync(x=>x.IdUsuario==u && x.IdPresupuesto==id,ct);
}
