namespace MoneyTrack.Domain;

public sealed class Categoria
{
    public int IdCategoria { get; set; }
    public int IdUsuario { get; set; }
    public required string Nombre { get; set; }
    public required string Tipo { get; set; }
}
