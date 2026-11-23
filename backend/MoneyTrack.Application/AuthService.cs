using System.ComponentModel.DataAnnotations;
using System.Text;
using MoneyTrack.Domain;

namespace MoneyTrack.Application;

public record Registro([param: Required, MaxLength(120)] string Nombre,
    [param: Required, EmailAddress, MaxLength(254)] string Correo,
    [param: Required, MinLength(8), MaxLength(72)] string Password);
public record Login([param: Required, EmailAddress, MaxLength(254)] string Correo,
    [param: Required, MaxLength(72)] string Password);
public record Perfil(int IdUsuario, string Nombre, string Correo, string Rol)
{
    public static Perfil De(Usuario u) => new(u.IdUsuario, u.Nombre, u.Correo, u.Rol);
}
public interface IUsuarios
{
    Task<Usuario?> Buscar(string correo, CancellationToken ct);
    Task<Usuario?> Buscar(int id, CancellationToken ct);
    Task<bool> Agregar(Usuario usuario, CancellationToken ct);
}
public sealed class AuthService(IUsuarios usuarios)
{
    private static readonly string DummyHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());
    public async Task<Perfil?> Registrar(Registro datos, CancellationToken ct)
    {
        var usuario = new Usuario { Nombre = datos.Nombre.Trim(), Correo = datos.Correo.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(datos.Password), Rol = "Usuario", FechaRegistro = DateTime.UtcNow };
        return await usuarios.Agregar(usuario, ct) ? Perfil.De(usuario) : null;
    }
    public async Task<Perfil?> Autenticar(Login datos, CancellationToken ct)
    {
        var usuario = await usuarios.Buscar(datos.Correo.Trim().ToLowerInvariant(), ct);
        var valido = Encoding.UTF8.GetByteCount(datos.Password) <= 72 &&
            BCrypt.Net.BCrypt.Verify(datos.Password, usuario?.PasswordHash ?? DummyHash);
        return valido && usuario != null ? Perfil.De(usuario) : null;
    }
    public async Task<Perfil?> PerfilActual(int id, CancellationToken ct) =>
        await usuarios.Buscar(id, ct) is Usuario usuario ? Perfil.De(usuario) : null;
}
