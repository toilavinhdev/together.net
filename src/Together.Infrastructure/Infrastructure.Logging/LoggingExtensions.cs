using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Infrastructure.Logging;

public static class LoggingExtensions
{
    public static void SetupSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_ , configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });
        
        builder.Logging.ClearProviders();
        
        builder.Logging.AddSerilog();
    }
}