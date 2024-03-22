using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ChatAPI.Extensions;

public static class AddJwtAuthenticationExtension
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.RequireHttpsMetadata = false;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Constants.Jwt.Issuer,
                    ValidAudience = Constants.Jwt.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Constants.Jwt.Key)
                };
            });
        
        return services;
    }
}