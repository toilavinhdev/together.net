namespace Infrastructure.Redis.Events;

public class SendNotificationSocketEvent : RedisPubSubEvent
{
    public List<string> SocketIds { get; set; } = default!;
    
    public Guid NotificationId { get; set; }
    
    public Guid SubjectId { get; set; }
    
    public string? SubjectAvatar { get; set; }

    public string SubjectUserName { get; set; } = default!;
    
    public bool SubjectIsOfficial { get; set; }
    
    public int NotificationType { get; set; }
    
    public Guid DirectObjectId { get; set; }
    
    public Guid? IndirectObjectId { get; set; }
    
    public Guid? PrepositionalObjectId { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}