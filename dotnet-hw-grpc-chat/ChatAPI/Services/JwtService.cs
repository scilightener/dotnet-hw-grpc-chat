using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grpc.Core;
using Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ChatAPI.Services;

public class JwtService : Jwt.JwtService.JwtServiceBase
{
    public override Task<Response> GetJwt(Request request, ServerCallContext context)
    {
        var jwt = GenerateJwtToken(request.Username);
        return Task.FromResult(new Response { Token = jwt });
    }

    private static string? GenerateJwtToken(string username)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: JwtConstants.Issuer,
            audience: JwtConstants.Audience,
            notBefore: now,
            claims: GetIdentity(username).Claims,
            expires: now + TimeSpan.FromMinutes(10),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(JwtConstants.Key),
                SecurityAlgorithms.HmacSha256));
    
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    private static ClaimsIdentity GetIdentity(string username)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, username),
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }
}