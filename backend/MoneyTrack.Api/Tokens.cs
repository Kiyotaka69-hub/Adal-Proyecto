using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoneyTrack.Application;

namespace MoneyTrack.Api;
public sealed class Tokens(IConfiguration config)
{
    public TokenValidationParameters Validation { get; } = new()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] is { Length: >= 32 } key ? key : throw new InvalidOperationException("Configure Jwt:Key con al menos 32 caracteres."))),
        ValidIssuer = config["Jwt:Issuer"] ?? "MoneyTrack", ValidAudience = config["Jwt:Audience"] ?? "MoneyTrack.Android",
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256], ClockSkew = TimeSpan.Zero, RoleClaimType = "role"
    };
    public object Emitir(Perfil perfil)
    {
        var expira = DateTime.UtcNow.AddHours(1);
        var jwt = new JwtSecurityToken(Validation.ValidIssuer, Validation.ValidAudience,
            [new Claim("sub", perfil.IdUsuario.ToString()), new Claim("role", perfil.Rol)],
            expires: expira, signingCredentials: new SigningCredentials(Validation.IssuerSigningKey, SecurityAlgorithms.HmacSha256));
        return new { token = new JwtSecurityTokenHandler().WriteToken(jwt), expira, usuario = perfil };
    }
}
