namespace MoneyTrack.Domain;

public sealed class Prediccion
{
    public int IdPrediccion { get; set; }
    public int IdUsuario { get; set; }
    public int IdCategoria { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    public decimal MontoPredicho { get; set; }
    public decimal Confianza { get; set; }
    public required string Metodo { get; set; }
    public DateTime FechaGenerada { get; set; }
}
