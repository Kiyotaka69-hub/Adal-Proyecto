using MoneyTrack.Application;
namespace MoneyTrack.Tests;

public class PrediccionTests
{
    [Fact] public void SinHistorialNoInventaGastos() =>
        Assert.Null(PrediccionService.Calcular(new decimal?[12]));
    [Fact] public void SerieConstanteTieneCoberturaCompleta()
    {
        var e=PrediccionService.Calcular(Enumerable.Repeat<decimal?>(100,12).ToArray());
        Assert.Equal(new Estimacion(100,1,105),e);
    }
    [Fact] public void UnMesTieneConfianzaLimitada()
    {
        var meses=new decimal?[12];
        meses[11]=80;
        Assert.Equal(new Estimacion(80,0.0833m,84),PrediccionService.Calcular(meses));
    }
    [Fact] public void AtipicoReduceConfianzaYLimitaLaEstimacion()
    {
        var meses=Enumerable.Repeat<decimal?>(100,12).ToArray();
        meses[11]=10000;
        var e=PrediccionService.Calcular(meses)!;
        Assert.InRange(e.Monto,100,200);
        Assert.InRange(e.Confianza,0,0.2m);
    }
}
