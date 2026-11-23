using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyTrack.Application;
namespace MoneyTrack.Api.Controllers;

[ApiController, Authorize, Route("api/categorias")]
public sealed class CategoriasController(FinanzasService service) : ControllerBase
{
    private int Usuario => int.Parse(User.FindFirst("sub")!.Value);
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await service.Categorias(Usuario,ct));
    [HttpPost]
    public async Task<IActionResult> Crear(DatosCategoria datos,CancellationToken ct)
    {
        var entidad=await service.GuardarCategoria(Usuario,null,datos,ct);
        return Created("/api/categorias",entidad);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id,DatosCategoria datos,CancellationToken ct) =>
        Ok(await service.GuardarCategoria(Usuario,id,datos,ct));
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id,CancellationToken ct)
    {
        await service.EliminarCategoria(Usuario,id,ct);
        return NoContent();
    }
}
