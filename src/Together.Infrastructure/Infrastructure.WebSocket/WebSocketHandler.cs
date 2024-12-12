using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Infrastructure.WebSocket;
using WebSocket = System.Net.WebSockets.WebSocket;

public abstract class WebSocketHandler
{
    public ConnectionManager ConnectionManager { get; set; }

    protected WebSocketHandler(ConnectionManager connectionManager)
    {
        ConnectionManager = connectionManager;
    }

    protected abstract Task ReceiveAsync(string socketId, WebSocket socket, WebSocketMessage message);

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
        var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);

        var socketId = ConnectionManager.GetId(socket);

        if (string.IsNullOrEmpty(socketId)) return;

        var message = JsonSerializer.Deserialize<WebSocketMessage>(messageJson) ?? new WebSocketMessage();

        await ReceiveAsync(socketId, socket, message);
    }

    public virtual Task OnConnectedAsync(string id, WebSocket socket)
    {
        ConnectionManager.AddSocket(id, socket);
        return Task.CompletedTask;
    }

    public virtual async Task OnDisconnectedAsync(string id, WebSocket socket)
    {
        await ConnectionManager.RemoveSocketAsync(socket);
    }
    
    public async Task SendMessageAsync(IEnumerable<string> socketIds, WebSocketMessage message)
    {
        foreach (var socketId in socketIds)
        {
            await SendMessageAsync(socketId, message);
        }
    }
    
    public async Task SendMessageAsync(string socketId, WebSocketMessage message)
    {
        var sockets = ConnectionManager.GetSockets(socketId);
        if (sockets is null) return;

        foreach (var socket in sockets)
        {
            var jsonMessage = JsonSerializer.Serialize(message, JsonSerializerOptions);
            await SendMessageAsync(socket, jsonMessage);
        }
    }
    
    private static async Task SendMessageAsync(WebSocket socket, string message)
    {
        if (socket.State != WebSocketState.Open)
            return;

        await socket.SendAsync(
            buffer: new ArraySegment<byte>(
                array: Encoding.UTF8.GetBytes(message),
                offset: 0,
                count: message.Length),
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
    }
    
    public async Task SendMessageBroadcastAsync(string message)
    {
        foreach (var pair in ConnectionManager.GetAll())
        {
            foreach (var socket in pair.Value.Where(socket => socket.State == WebSocketState.Open))
            {
                await SendMessageAsync(socket, message);
            }
        }
    }
}