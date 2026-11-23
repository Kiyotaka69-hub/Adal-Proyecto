using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoneyTrack.Application;
using MoneyTrack.Domain;
namespace MoneyTrack.Infrastructure;

public sealed class UsuarioRepository(MoneyTrackContext db) : IUsuarios
{
    public Task<Usuario?> Buscar(string correo, CancellationToken ct) =>
        db.Usuarios.AsNoTracking().SingleOrDefaultAsync(u => u.Correo == correo, ct);
    public Task<Usuario?> Buscar(int id, CancellationToken ct) =>
        db.Usuarios.AsNoTracking().SingleOrDefaultAsync(u => u.IdUsuario == id, ct);
    public async Task<bool> Agregar(Usuario usuario, CancellationToken ct)
    {
        db.Usuarios.Add(usuario);
        try { await db.SaveChangesAsync(ct); return true; }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        { return false; }
    }
}
