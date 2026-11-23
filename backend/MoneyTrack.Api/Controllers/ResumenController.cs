using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using MoneyTrack.Application;
namespace MoneyTrack.Api.Controllers;

public sealed class Periodo
{
    [Range(1,12)] public int Mes {get;set;}=DateTime.Today.Month;
    [Range(1,9999)] public int Anio {get;set;}=DateTime.Today.Year;
}
[ApiController,Authorize,Route("api")]
public sealed class ResumenController(PresupuestosService service):ControllerBase
{
    private int Usuario=>int.Parse(User.FindFirst("sub")!.Value);
    [HttpGet("dashboard/resumen")]
    public async Task<IActionResult> Resumen([FromQuery] Periodo p,CancellationToken ct)=>Ok(await service.Resumen(Usuario,p.Mes,p.Anio,ct));
    [HttpGet("dashboard/estadisticas")]
    public async Task<IActionResult> Estadisticas([FromQuery] Periodo p,CancellationToken ct)=>Ok(await service.Estadisticas(Usuario,p.Mes,p.Anio,ct));
    [HttpGet("presupuestos")]
    public async Task<IActionResult> Listar([FromQuery] Periodo p,CancellationToken ct)=>Ok(await service.Avances(Usuario,p.Mes,p.Anio,ct));
    [HttpPost("presupuestos")]
    public async Task<IActionResult> Crear(DatosPresupuesto datos,CancellationToken ct)=>
        StatusCode(201,await service.Guardar(Usuario,null,datos,ct));
    [HttpPut("presupuestos/{id:int}")]
    public async Task<IActionResult> Editar(int id,DatosPresupuesto datos,CancellationToken ct)=>Ok(await service.Guardar(Usuario,id,datos,ct));
    [HttpDelete("presupuestos/{id:int}")]
    public async Task<IActionResult> Eliminar(int id,CancellationToken ct)
    {
        await service.Eliminar(Usuario,id,ct);
        return NoContent();
    }
}
