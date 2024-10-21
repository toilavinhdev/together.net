using Infrastructure.SharedKernel;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.WebSocket;
using Service.Socket;
using Service.Socket.BackgroundServices;

var (builder, appSettings) = WebApplicationBuilderExtensions.CreateCoreBuilder<AppSettings>(args);

var services = builder.Services;
services.AddSharedKernel<Program>(appSettings);
services.AddWebSocketHandlers<Program>();
services.AddHostedService<SendMessageSocketBackgroundService>();
services.AddHostedService<SendNotificationSocketBackgroundService>();

var app = builder.Build();
app.UseSharedKernel(appSettings);
app.UseGrpc(appSettings.GrpcEndpoints.ServiceSocket, _ => {});
app.UseWebSockets();
app.MapWebSocketHandler<SocketHandler>($"/{appSettings.Metadata.EndpointPrefix}/ws");
app.MapGet("/socket/connections", (SocketHandler handler) => handler.ConnectionManager.GetAll());
app.Run();