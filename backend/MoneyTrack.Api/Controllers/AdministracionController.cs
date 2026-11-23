using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyTrack.Application;
namespace MoneyTrack.Api.Controllers;

public record CambioRol([Required,RegularExpression("Administrador|Usuario")] string Rol);
[ApiController,Authorize(Roles="Administrador"),Route("api/admin/usuarios")]
public sealed class AdministracionController(AdministracionService service):ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery,Range(1,1000000)] int pagina=1,CancellationToken ct=default)=>
        Ok(await service.Listar(pagina,ct));
    [HttpPut("{id:int}/rol")]
    public async Task<IActionResult> Cambiar(int id,CambioRol datos,CancellationToken ct)
    {
        await service.CambiarRol(int.Parse(User.FindFirst("sub")!.Value),id,datos.Rol,ct);
        return NoContent();
    }
}
