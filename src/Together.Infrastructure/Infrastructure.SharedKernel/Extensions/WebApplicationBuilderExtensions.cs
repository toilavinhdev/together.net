using Infrastructure.Logging;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.SharedKernel.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static (WebApplicationBuilder, TSetting) CreateCoreBuilder<TSetting>(string[] args) where TSetting : BaseSettings, new()
    {
        var builder = WebApplication.CreateBuilder(args);
        var settings = GetEnvironment<TSetting>(builder);
        builder.Services.AddSingleton(new BaseSettings
        {
            Metadata = settings.Metadata,
            LoggingConfig = settings.LoggingConfig,
            GrpcEndpoints = settings.GrpcEndpoints,
            JwtConfig = settings.JwtConfig,
            RedisConfiguration = settings.RedisConfiguration,
            RabbitMqConfig = settings.RabbitMqConfig
        });
        builder.Services.AddSingleton(settings);
        LoggingExtensions.ConfigureLogging(builder, settings.LoggingConfig, settings.Metadata.Name);
        return (builder, settings);
    }
    
    private static TEnvironment GetEnvironment<TEnvironment>(WebApplicationBuilder builder) 
        where TEnvironment : BaseSettings, new()
    {
        var settings = new TEnvironment();
        var environment = builder.Environment;
        var json = $"appsettings.{environment.EnvironmentName}.json";
        var path = Path.Combine(typeof(TEnvironment).Name, json);
        new ConfigurationBuilder()
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile(path)
            .AddEnvironmentVariables()
            .Build()
            .Bind(settings);
        return settings;
    }
}