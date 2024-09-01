using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.SharedKernel.Extensions;

public static class EnvironmentExtensions
{
    public static void SetupEnvironment<TEnvironment>(this WebApplicationBuilder builder,
        out TEnvironment settings) where TEnvironment : BaseSettings, new()

    {
        settings = new TEnvironment();
        var environment = builder.Environment;
        var json = $"appsettings.{environment.EnvironmentName}.json";
        var path = Path.Combine(typeof(TEnvironment).Name, json);

        new ConfigurationBuilder()
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile(path)
            .AddEnvironmentVariables()
            .Build()
            .Bind(settings);

        builder.Services.AddSingleton(settings);
    }
}