namespace Infrastructure.Redis.Events;

public class SendMessageSocketEvent : RedisPubSubEvent
{
    public List<string> SocketIds { get; set; } = default!;
    
    public Guid ConversationId { get; set; }
    
    public string Text { get; set; } = default!;
    
    public Guid CreatedById { get; set; }
    
    public string CreatedByUserName { get; set; } = default!;

    public string? CreatedByAvatar { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}