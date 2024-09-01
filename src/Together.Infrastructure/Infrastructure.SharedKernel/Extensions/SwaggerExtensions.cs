using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using static System.String;

namespace Infrastructure.SharedKernel.Extensions;

public static class SwaggerExtensions
{
    private const string DocumentName = "v1";

    public static void AddCoreSwagger(this IServiceCollection services, string title)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            o =>
            {
                o.SwaggerDoc(DocumentName, new OpenApiInfo
                {
                    Title = title,
                    Version = "v1",
                    Description = $"Summary of APIs of Together.NET forum - {title}"
                });
                
                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. " +
                                  "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                                  "\r\n\r\nExample: \"Bearer accessToken\"",
                });
                
                o.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, []
                    }
                });
            });
    }
    
    public static void UseCoreSwagger(this WebApplication app, string title, string? prefix = null)
    {
        const string template = "/swagger/{documentName}/swagger.json";
        prefix = IsNullOrEmpty(prefix) ? "swagger" : $"{prefix}/swagger";
        app.UseSwagger(o =>
        {
            o.RouteTemplate = template;
        });
        app.UseSwaggerUI(o =>
        {
            o.DocumentTitle = title;
            o.RoutePrefix = prefix;
            o.SwaggerEndpoint(template.Replace("{documentName}", DocumentName), DocumentName);
        });
    }
}