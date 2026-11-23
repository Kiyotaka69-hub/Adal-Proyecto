using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoneyTrack.Application;
using System.ComponentModel.DataAnnotations;
namespace MoneyTrack.Api.Controllers;

public class PeriodoPrediccion
{
    [Range(2,9999)] public int Anio { get; set; }
    [Range(1,12)] public int Mes { get; set; }
    public DateOnly Fecha() => new(Anio,Mes,1);
}
public sealed class Confirmacion : PeriodoPrediccion
{
    public int IdCategoria {get;set;}
    public bool Confirmado {get;set;}
    public decimal MontoConfirmado {get;set;}
}
public sealed class Pregunta : PeriodoPrediccion
{
    [Required,MaxLength(1000)] public string Mensaje {get;set;}="";
}
[Authorize,ApiController,Route("api")]
public sealed class AsistenteController(PrediccionService service):ControllerBase
{
    private int Usuario=>int.Parse(User.FindFirst("sub")!.Value);
    [HttpGet("asistente/prediccion")]
    public async Task<IActionResult> Prediccion([FromQuery] PeriodoPrediccion p,CancellationToken ct) =>
        Ok(new { p.Mes,p.Anio,metodo=PrediccionService.Metodo,aviso=PrediccionService.Aviso,categorias=await service.Predecir(Usuario,p.Fecha(),ct) });
    [HttpPost("presupuestos/sugerencia")]
    public async Task<IActionResult> Sugerencia(PeriodoPrediccion p,CancellationToken ct) =>
        Ok(new { requiereConfirmacion=true,aviso=PrediccionService.Aviso,categorias=await service.Predecir(Usuario,p.Fecha(),ct) });
    [HttpPost("presupuestos/sugerencia/confirmar")]
    public async Task<IActionResult> Confirmar(Confirmacion datos,[FromServices] PresupuestosService presupuestos,CancellationToken ct)
    {
        if(!datos.Confirmado) return Problem(statusCode:400,detail:"Confirma explícitamente la sugerencia.");
        return StatusCode(201,await presupuestos.AplicarSugerencia(Usuario,datos.IdCategoria,datos.Fecha(),datos.MontoConfirmado,service,ct));
    }
    [HttpPost("asistente/chat")]
    public async Task<IActionResult> Chat(Pregunta pregunta,CancellationToken ct) =>
        Ok(new { modo="local",respuesta=await service.Chat(Usuario,pregunta.Mensaje,pregunta.Fecha(),ct) });
    [HttpGet("asistente/chat")]
    public async Task<IActionResult> Historial(CancellationToken ct)=>Ok(await service.Historial(Usuario,ct));
}
