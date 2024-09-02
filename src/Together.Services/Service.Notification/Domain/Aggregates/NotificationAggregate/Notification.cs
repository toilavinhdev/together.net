using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.Enums;
using Service.Notification.Domain.Enums;

namespace Service.Notification.Domain.Aggregates.NotificationAggregate;

public sealed class Notification : TimeTrackingEntity
{
    public Guid ReceiverId { get; set; }
    
    public Guid SubjectId { get; set; }
    
    public NotificationType Type { get; set; }
    
    public Guid DirectObjectId { get; set; }
    
    public Guid? IndirectObjectId { get; set; }
    
    public Guid? PrepositionalObjectId { get; set; }
    
    public NotificationStatus Status { get; set; }
}