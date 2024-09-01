namespace Infrastructure.WebSocket;

public class WebSocketMessage
{
    public string Target { get; set; } = default!;

    public object? Message { get; set; }
}