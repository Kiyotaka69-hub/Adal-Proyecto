namespace MoneyTrack.Domain;

public sealed class Usuario
{
    public int IdUsuario { get; set; }
    public required string Nombre { get; set; }
    public required string Correo { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime FechaRegistro { get; set; }
    public required string Rol { get; set; }
}
