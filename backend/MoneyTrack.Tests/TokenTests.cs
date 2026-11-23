using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoneyTrack.Api;
using MoneyTrack.Application;
namespace MoneyTrack.Tests;

public class TokenTests
{
    private static Tokens Crear() => new(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>
    { ["Jwt:Key"]=Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48)) }).Build());
    [Fact] public void TokenExpiradoSeRechaza()
    {
        var parametros=Crear().Validation;
        var jwt=new JwtSecurityToken(parametros.ValidIssuer,parametros.ValidAudience,expires:DateTime.UtcNow.AddMinutes(-1),
            signingCredentials:new SigningCredentials(parametros.IssuerSigningKey,SecurityAlgorithms.HmacSha256));
        var handler=new JwtSecurityTokenHandler();
        Assert.Throws<SecurityTokenExpiredException>(()=>handler.ValidateToken(handler.WriteToken(jwt),parametros,out _));
    }
}
