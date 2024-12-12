using Infrastructure.Logging;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var configurationRoot = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(Path.Combine("AppSettings", $"appsettings.{builder.Environment.EnvironmentName}.json"))
    .AddEnvironmentVariables()
    .Build();

LoggingExtensions.ConfigureLogging(
    builder: builder,
    loggingConfig: configurationRoot
                       .GetSection("LoggingConfig")
                       .Get<LoggingConfig>() 
                   ?? throw new NullReferenceException("Logging config is null"),
    applicationName: configurationRoot
                         .GetSection("Metadata")
                         .GetValue<string>("Name")
                     ?? throw new NullReferenceException("Metadata is null"));

var services = builder.Services;
services
    .AddReverseProxy()
    .LoadFromConfig(configurationRoot.GetSection("ReverseProxy"))
    .AddTransforms(b =>
    {
        b.AddRequestTransform(ctx =>
        {
            var correlationId = CorrelationIdExtensions.GenerateCorrelationId();
            ctx.ProxyRequest.Headers.AddCorrelationId(correlationId);
            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();
app.MapReverseProxy();
app.MapGet("/ping", () => "Gateway.Yarp - Pong");
app.Run();