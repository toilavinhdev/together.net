using System.Text;
using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.SharedKernel.Extensions;

public static class AuthExtensions
{
    public static void AddCoreAuth(this IServiceCollection services, JwtConfig config)
    {
        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
                o =>
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.TokenSigningKey));

                    o.TokenValidationParameters.IssuerSigningKey = key;
                    o.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    o.TokenValidationParameters.ValidateLifetime = true;
                    o.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                    o.TokenValidationParameters.ValidAudience = config.Audience;
                    o.TokenValidationParameters.ValidateAudience = config.Audience is not null;
                    o.TokenValidationParameters.ValidIssuer = config.Issuer;
                    o.TokenValidationParameters.ValidateIssuer = config.Audience is not null;
                });
        
        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        
        services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
        
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultHandler>();
        
        services.AddAuthorization();
    }

    public static void UseCoreAuth(this WebApplication app)
    {
        app.UseAuthentication();
        
        app.UseAuthorization();
    }
}