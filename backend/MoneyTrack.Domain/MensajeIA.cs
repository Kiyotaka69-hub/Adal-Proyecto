namespace MoneyTrack.Domain;

public sealed class MensajeIA
{
    public int IdMensaje { get; set; }
    public int IdUsuario { get; set; }
    public required string Rol { get; set; }
    public required string Contenido { get; set; }
    public DateTime Fecha { get; set; }
}
