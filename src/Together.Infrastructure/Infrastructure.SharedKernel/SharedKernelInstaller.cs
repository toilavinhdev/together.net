using Infrastructure.Logging;
using Infrastructure.RabbitMQ;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.SharedKernel;

public static class SharedKernelInstaller
{
    public static void AddSharedKernel<TAssembly>(this IServiceCollection services, BaseSettings baseSettings)
    {
        services.AddHttpContextAccessor();
        
        services.AddCoreCors(baseSettings.Metadata.Name);
        
        services.AddCoreAuth(baseSettings.JwtConfig);
        
        services.AddCoreLanguages();
        
        services.AddEndpoints(typeof(TAssembly).Assembly);
        
        services.AddCoreSwagger(baseSettings.Metadata.Name);
        
        services.AddCoreMapper(typeof(TAssembly).Assembly);
        
        services.AddCoreMediator(typeof(TAssembly).Assembly);
        
        services.AddRedis(baseSettings.RedisConfiguration);
        
        services.AddRabbitMQ(baseSettings.RabbitMqConfig);
    }

    public static void UseSharedKernel(this WebApplication app, BaseSettings baseSettings)
    {
        app.UseCoreCors(baseSettings.Metadata.Name);
        
        app.UseLanguages();
        
        app.UseCoreExceptionHandler();
        
        app.UseCoreAuth();

        app.UseCorrelationId();
        
        app.UseCoreSwagger(baseSettings.Metadata.Name, baseSettings.Metadata.EndpointPrefix);
        
        app.MapEndpoints(app.MapGroup(baseSettings.Metadata.EndpointPrefix));
        
        app.MapGet($"{baseSettings.Metadata.EndpointPrefix}/ping", () => $"{baseSettings.Metadata.Name} - Pong");
    }
}