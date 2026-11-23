namespace MoneyTrack.Domain;

public sealed class Movimiento
{
    public int IdMovimiento { get; set; }
    public int IdUsuario { get; set; }
    public int IdCategoria { get; set; }
    public decimal Cantidad { get; set; }
    public required string Tipo { get; set; }
    public string? Descripcion { get; set; }
    public DateOnly Fecha { get; set; }
    public bool EsRecurrente { get; set; }
}
