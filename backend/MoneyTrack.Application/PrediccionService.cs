using System.Globalization;
namespace MoneyTrack.Application;

public record MesCategoria(int IdCategoria, int Anio, int Mes, decimal Total);
public record CategoriaGasto(int IdCategoria, string Nombre);
public record Estimacion(decimal Monto, decimal Confianza, decimal Sugerido);
public record Pronostico(int IdCategoria, string Categoria, Estimacion? Estimacion, int MesesObservados, bool Riesgo);
public record LineaChat(string Rol, string Contenido, DateTime Fecha);
public interface IPrediccionRepository
{
    Task<List<MesCategoria>> Totales(int usuario, DateOnly desde, DateOnly hasta, CancellationToken ct);
    Task<List<CategoriaGasto>> Categorias(int usuario, CancellationToken ct);
    Task Guardar(int usuario, DateOnly periodo, IReadOnlyList<Pronostico> filas, CancellationToken ct);
    Task Conversar(int usuario, string pregunta, string respuesta, CancellationToken ct);
    Task<List<LineaChat>> Historial(int usuario, CancellationToken ct);
}
public sealed class PrediccionService(IPrediccionRepository repo, IResumenRepository resumen)
{
    public const string Metodo = "MMP-tendencia-v1";
    public const string Aviso = "Estimación orientativa. La confianza mide cobertura y estabilidad del historial. No es una probabilidad calibrada ni asesoría financiera profesional.";
    private const decimal Maximo = 9999999999.99m;
    public static Estimacion? Calcular(IReadOnlyList<decimal?> meses)
    {
        var observados = meses.OfType<decimal>().Order().ToArray();
        if (observados.Length == 0) return null;
        var mediana = (observados[(observados.Length-1)/2] + observados[observados.Length/2])/2;
        var tope = observados.Length >= 4 ? mediana * 3 : decimal.MaxValue;
        decimal suma = 0, pesos = 0, dispersion = 0;
        var primero = -1;
        var ultimo = -1;
        for (int i = 0; i < meses.Count; i++)
        {
            if (meses[i] is not decimal valor) continue;
            suma += Math.Min(valor,tope)*(i+1);
            pesos += i+1;
            dispersion += Math.Abs(valor-mediana);
            if (primero < 0) primero = i;
            ultimo = i;
        }
        var media = suma/pesos;
        var pendiente = ultimo == primero ? 0 :
            (Math.Min(meses[ultimo]!.Value,tope)-Math.Min(meses[primero]!.Value,tope))/(ultimo-primero);
        var monto = decimal.Round(Math.Clamp(media+Math.Clamp(pendiente,-media/4,media/4),0,Maximo),2);
        var confianza = decimal.Round(observados.Length/(decimal)meses.Count /
            (1+dispersion/observados.Length/Math.Max(1,mediana)),4);
        return new(monto,confianza,Math.Min(Maximo,decimal.Round(monto*1.05m,2)));
    }
    public async Task<List<Pronostico>> Predecir(int usuario, DateOnly periodo, CancellationToken ct)
    {
        var desde = periodo.AddMonths(-12);
        var totales = (await repo.Totales(usuario,desde,periodo,ct)).ToDictionary(x=>(x.IdCategoria,x.Anio,x.Mes),x=>x.Total);
        var avances = (await resumen.Avances(usuario,periodo.Month,periodo.Year,ct)).ToDictionary(x=>x.IdCategoria);
        var filas = new List<Pronostico>();
        foreach (var cat in await repo.Categorias(usuario,ct))
        {
            var meses = Enumerable.Range(0,12).Select(i => {
                var fecha = desde.AddMonths(i);
                return totales.TryGetValue((cat.IdCategoria,fecha.Year,fecha.Month),out var total) ? (decimal?)total : null;
            }).ToArray();
            var estimacion = Calcular(meses);
            var riesgo = avances.TryGetValue(cat.IdCategoria,out var avance) &&
                (avance.Gastado > avance.Limite || estimacion?.Monto > avance.Limite);
            filas.Add(new(cat.IdCategoria,cat.Nombre,estimacion,meses.Count(x=>x.HasValue),riesgo));
        }
        await repo.Guardar(usuario,periodo,filas,ct);
        return filas;
    }
    public Task<List<LineaChat>> Historial(int usuario,CancellationToken ct) => repo.Historial(usuario,ct);
    public async Task<string> Chat(int usuario,string pregunta,DateOnly periodo,CancellationToken ct)
    {
        var texto = pregunta.ToLowerInvariant();
        var moneda = CultureInfo.GetCultureInfo("es-MX");
        string respuesta;
        if (texto.Contains("saldo"))
            respuesta = $"Tu saldo registrado es {(await resumen.Saldo(usuario,ct)).ToString("C2",moneda)} MXN.";
        else if (texto.Contains("gasto") || texto.Contains("ingreso"))
        {
            var tipo = texto.Contains("ingreso") ? "Ingreso" : "Gasto";
            var total = (await resumen.Estadisticas(usuario,periodo.Month,periodo.Year,ct)).Where(x=>x.Tipo==tipo).Sum(x=>x.Total);
            respuesta = $"{tipo} registrado en {periodo:MM/yyyy}: {total.ToString("C2",moneda)} MXN.";
        }
        else if (texto.Contains("presupuesto") || texto.Contains("predic"))
        {
            var filas = await Predecir(usuario,periodo,ct);
            var estimadas = filas.Where(x=>x.Estimacion!=null).ToList();
            respuesta = estimadas.Count==0 ? "Falta historial anterior al periodo para estimar gastos." :
                $"Para {periodo:MM/yyyy} estimo {estimadas.Sum(x=>x.Estimacion!.Monto).ToString("C2",moneda)} MXN de gasto entre {estimadas.Count} categorías. Uso una media ponderada con tendencia. {Aviso}";
        }
        else respuesta = "El asistente local consulta saldo, ingresos, gastos y predicción por periodo. Puedes preguntar cuánto gastaste.";
        await repo.Conversar(usuario,pregunta,respuesta,ct);
        return respuesta;
    }
}
