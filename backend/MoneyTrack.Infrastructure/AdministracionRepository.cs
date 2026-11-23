using Microsoft.EntityFrameworkCore;
using MoneyTrack.Application;
namespace MoneyTrack.Infrastructure;

public sealed class AdministracionRepository(MoneyTrackContext db) : IAdministracionRepository
{
    public Task<List<Perfil>> Listar(int pagina,CancellationToken ct) => db.Usuarios.AsNoTracking()
        .OrderBy(x=>x.IdUsuario).Skip((pagina-1)*50).Take(50)
        .Select(x=>new Perfil(x.IdUsuario,x.Nombre,x.Correo,x.Rol)).ToListAsync(ct);
    public async Task<bool> CambiarRol(int id,string rol,CancellationToken ct)
    {
        await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable,ct);
        if(rol=="Usuario" && await db.Usuarios.CountAsync(x=>x.Rol=="Administrador",ct)<=1 &&
            await db.Usuarios.AnyAsync(x=>x.IdUsuario==id && x.Rol=="Administrador",ct))
            throw new FinanzasException(409,"Se requiere un administrador activo.");
        var filas=await db.Usuarios.Where(x=>x.IdUsuario==id).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.Rol,rol),ct);
        await tx.CommitAsync(ct);
        return filas==1;
    }
}
