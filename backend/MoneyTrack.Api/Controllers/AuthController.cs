using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MoneyTrack.Application;
using System.Text;

namespace MoneyTrack.Api.Controllers;

[ApiController, Route("api/auth")]
public sealed class AuthController(AuthService auth, Tokens tokens) : ControllerBase
{
    [HttpPost("register"), AllowAnonymous, EnableRateLimiting("auth")]
    public async Task<IActionResult> Registrar(Registro datos, CancellationToken ct)
    {
        if (Encoding.UTF8.GetByteCount(datos.Password) > 72)
            return Problem(statusCode: 400, detail: "La contraseña excede 72 bytes UTF-8.");
        var perfil = await auth.Registrar(datos, ct);
        return perfil == null ? Conflict() : Created("/api/auth/me", perfil);
    }
    [HttpPost("login"), AllowAnonymous, EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(Login datos, CancellationToken ct) =>
        await auth.Autenticar(datos, ct) is Perfil perfil ? Ok(tokens.Emitir(perfil)) : Unauthorized();

    [HttpGet("me"), Authorize]
    public async Task<IActionResult> Me(CancellationToken ct) =>
        int.TryParse(User.FindFirst("sub")?.Value, out var id) && await auth.PerfilActual(id, ct) is Perfil perfil
            ? Ok(perfil) : Unauthorized();
}
