using Infrastructure.Redis;
using Infrastructure.Redis.Events;
using Infrastructure.WebSocket;

namespace Service.Socket.BackgroundServices;

public sealed class SendNotificationSocketBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IRedisService _redisService = serviceProvider.GetRequiredService<IRedisService>();
    
    private readonly SocketHandler _socketHandler = serviceProvider.GetRequiredService<SocketHandler>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _redisService.SubscribeAsync<SendNotificationSocketEvent>(async @event =>
        {
            await _socketHandler.SendMessageAsync(@event.SocketIds, new WebSocketMessage
            {
                Target = WebSocketTarget.Client.ReceivedNotification,
                Message = @event
            });
        });
    }
}