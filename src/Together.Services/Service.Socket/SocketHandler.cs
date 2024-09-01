using System.Net.WebSockets;
using Infrastructure.Redis;
using Infrastructure.WebSocket;

namespace Service.Socket;

public sealed class SocketHandler(ConnectionManager connectionManager, IRedisService redisService) 
    : WebSocketHandler(connectionManager)
{
    public override async Task OnConnectedAsync(string id, WebSocket socket)
    {
        await base.OnConnectedAsync(id, socket);
        await redisService.SetAddAsync(RedisKeys.SocketOnlineUsers(), id);
    }

    public override async Task OnDisconnectedAsync(string id, WebSocket socket)
    {
        await base.OnDisconnectedAsync(id, socket);
        await redisService.SetRemoveAsync(RedisKeys.SocketOnlineUsers(), id);
    }

    protected override async Task ReceiveAsync(string socketId, WebSocket socket, WebSocketMessage message)
    {
        switch (message.Target)
        {
            case WebSocketTarget.Server.Ping:
                break;
        }
        
        await Task.CompletedTask;
    }
}