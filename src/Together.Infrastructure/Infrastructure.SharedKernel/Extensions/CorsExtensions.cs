using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.SharedKernel.Extensions;

public static class CorsExtensions
{
    public static void AddCoreCors(this IServiceCollection services, string policyName)
    {
        services.AddCors(o =>
        {
            o.AddPolicy(policyName, b =>
            {
                b.AllowCredentials()
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .SetIsOriginAllowed(_ => true);
            });
        });
    }

    public static void UseCoreCors(this WebApplication app, string policyName)
    {
        app.UseCors(policyName);
    }
}