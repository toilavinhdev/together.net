using Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.SetupSerilog();

var configurationRoot = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(Path.Combine("AppSettings", $"appsettings.{builder.Environment.EnvironmentName}.json"))
    .AddEnvironmentVariables()
    .Build();

var services = builder.Services;
services.AddReverseProxy().LoadFromConfig(configurationRoot.GetSection("ReverseProxy"));

var app = builder.Build();
app.MapReverseProxy();
app.MapGet("/ping", () => "Gateway.Yarp - Pong");
app.Run();