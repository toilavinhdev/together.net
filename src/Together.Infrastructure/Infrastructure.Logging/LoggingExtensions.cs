using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Infrastructure.Logging;

public static class LoggingExtensions
{
    public static void ConfigureLogging(WebApplicationBuilder builder, 
        LoggingConfig loggingConfig,
        string applicationName)
    {
        builder.Host.UseSerilog((_ , configuration) =>
        {
            configuration
                .MinimumLevel.Information();
            configuration
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", applicationName);
            configuration
                .WriteTo.Async(c => c.Console())
                .WriteTo.Async(c => c.File(
                    path: loggingConfig.LogToFilePath, 
                    rollingInterval: RollingInterval.Day, 
                    restrictedToMinimumLevel: LogEventLevel.Error))
                .WriteTo.Async(c => c.Seq(loggingConfig.LogToSeqUrl));
        });
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
    }
}