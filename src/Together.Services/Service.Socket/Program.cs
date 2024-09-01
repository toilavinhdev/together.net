using Infrastructure.Logging;
using Infrastructure.SharedKernel;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.WebSocket;
using Service.Socket;

var builder = WebApplication.CreateBuilder(args);
builder.SetupEnvironment<AppSettings>(out var appSettings);
builder.SetupSerilog();

var services = builder.Services;
services.AddSharedKernel<Program>(appSettings);
services.AddWebSocketHandlers<Program>();

var app = builder.Build();
app.UseSharedKernel(appSettings);
app.UseWebSockets();
app.MapWebSocketHandler<SocketHandler>($"/{appSettings.Metadata.EndpointPrefix}/ws");
app.MapGet("/socket/connections", (SocketHandler handler) => handler.ConnectionManager.GetAll());
app.Run();