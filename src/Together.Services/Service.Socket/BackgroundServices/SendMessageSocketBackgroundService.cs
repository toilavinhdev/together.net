using Infrastructure.Redis;
using Infrastructure.Redis.Events;
using Infrastructure.WebSocket;

namespace Service.Socket.BackgroundServices;

public sealed class SendMessageSocketBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IRedisService _redisService = serviceProvider.GetRequiredService<IRedisService>();
    
    private readonly SocketHandler _socketHandler = serviceProvider.GetRequiredService<SocketHandler>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _redisService.SubscribeAsync<SendMessageSocketEvent>(async @event =>
        {
            await _socketHandler.SendMessageAsync(@event.SocketIds, new WebSocketMessage
            {
                Target = WebSocketTarget.Client.ReceivedMessage,
                Message = @event
            });
        });
    }
}