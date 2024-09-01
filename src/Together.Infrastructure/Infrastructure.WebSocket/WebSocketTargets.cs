namespace Infrastructure.WebSocket;

public class WebSocketTarget
{
    public class Client
    {
        public const string ReceivedNotification = nameof(ReceivedNotification);
    
        public const string ReceivedMessage = nameof(ReceivedMessage);
    }
    
    public class Server
    {
        public const string Ping = nameof(Ping);
    }
}