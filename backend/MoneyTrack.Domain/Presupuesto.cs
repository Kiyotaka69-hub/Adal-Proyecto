namespace MoneyTrack.Domain;

public sealed class Presupuesto
{
    public int IdPresupuesto { get; set; }
    public int IdUsuario { get; set; }
    public int IdCategoria { get; set; }
    public decimal Limite { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    public required string Origen { get; set; }
    public decimal? MontoSugerido { get; set; }
}
