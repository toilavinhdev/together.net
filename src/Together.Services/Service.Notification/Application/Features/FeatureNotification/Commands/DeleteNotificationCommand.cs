using Infrastructure.SharedKernel.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.Notification.Domain;

namespace Service.Notification.Application.Features.FeatureNotification.Commands;

public sealed class DeleteNotificationCommand : IRequest
{
    public Guid SubjectId { get; set; }
    
    public NotificationType Type { get; set; }
    
    public Guid DirectObjectId { get; set; }
    
    internal class Handler(NotificationContext context) : IRequestHandler<DeleteNotificationCommand>
    {
        public async Task Handle(DeleteNotificationCommand request, CancellationToken ct)
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(n =>
                        n.SubjectId == request.SubjectId &&
                        n.Type == request.Type &&
                        n.DirectObjectId == request.DirectObjectId,
                    ct);
            
            if (notification is null) return;

            context.Notifications.Remove(notification);

            await context.SaveChangesAsync(ct);
        }
    }
}