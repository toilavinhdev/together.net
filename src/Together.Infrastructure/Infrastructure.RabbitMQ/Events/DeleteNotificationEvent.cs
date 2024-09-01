namespace Infrastructure.RabbitMQ.Events;

public class DeleteNotificationEvent : RabbitMQEvent
{
    public Guid SubjectId { get; set; }
    
    public int NotificationType { get; set; }
    
    public Guid DirectObjectId { get; set; }
}