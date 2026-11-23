using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyTrack.Application;
namespace MoneyTrack.Api.Controllers;

[ApiController, Authorize, Route("api/movimientos")]
public sealed class MovimientosController(FinanzasService service) : ControllerBase
{
    private int Usuario => int.Parse(User.FindFirst("sub")!.Value);
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] FiltroMovimientos filtro, CancellationToken ct) =>
        Ok(await service.Historial(Usuario,filtro,ct));
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Consultar(int id,CancellationToken ct) => Ok(await service.Movimiento(Usuario,id,ct));
    [HttpPost]
    public async Task<IActionResult> Crear(DatosMovimiento datos,CancellationToken ct)
    {
        var entidad=await service.GuardarMovimiento(Usuario,null,datos,ct);
        return Created("/api/movimientos/"+entidad.IdMovimiento,entidad);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id,DatosMovimiento datos,CancellationToken ct) =>
        Ok(await service.GuardarMovimiento(Usuario,id,datos,ct));
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id,CancellationToken ct)
    {
        await service.EliminarMovimiento(Usuario,id,ct);
        return NoContent();
    }
}
