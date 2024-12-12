using Infrastructure.Redis;
using Infrastructure.Redis.Events;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Notification.Domain;
using Service.Notification.Domain.Enums;

namespace Service.Notification.Application.Features.FeatureNotification.Commands;
using Notification = Domain.Aggregates.NotificationAggregate.Notification;

public sealed class CreateNotificationCommand : IBaseRequest
{
    public Guid ReceiverId { get; set; }
    
    public Guid SubjectId { get; set; }
    
    public NotificationType Type { get; set; }
    
    public Guid DirectObjectId { get; set; }
    
    public Guid? IndirectObjectId { get; set; }
    
    public Guid? PrepositionalObjectId { get; set; }

    public string CorrelationId { get; set; } = default!;
    
    internal class Handler(
        NotificationContext context,
        IRedisService redisService,
        IHttpContextAccessor httpContextAccessor
    ) : BaseRequestHandler<CreateNotificationCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(CreateNotificationCommand request, CancellationToken ct)
        {
            var existedNotification = await context.Notifications
                .FirstOrDefaultAsync(n =>
                        n.SubjectId == request.SubjectId &&
                        n.Type == request.Type &&
                        n.DirectObjectId == request.DirectObjectId,
                    ct);
            
            if (existedNotification is not null)
            {
                existedNotification.Status = NotificationStatus.Unread;
                existedNotification.MarkCreated();
                
                context.Notifications.Update(existedNotification);
                
                await context.SaveChangesAsync(ct);
                
                return;
            }
            
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ReceiverId = request.ReceiverId,
                SubjectId = request.SubjectId,
                Type = request.Type,
                DirectObjectId = request.DirectObjectId,
                IndirectObjectId = request.IndirectObjectId,
                PrepositionalObjectId = request.PrepositionalObjectId,
                Status = NotificationStatus.Unread
            };
            notification.MarkCreated();

            await context.Notifications.AddAsync(notification, ct);

            await PublishSendNotificationSocketEvent(notification, request.CorrelationId);

            await context.SaveChangesAsync(ct);
        }
        
        private async Task PublishSendNotificationSocketEvent(Notification notification, string correlationId)
        {
            var subject = await redisService.StringGetAsync<IdentityPrivilege>(
                RedisKeys.Identity<IdentityPrivilege>(notification.SubjectId));
            
            if (subject is null) return;
            
            await redisService.PublishAsync(new SendNotificationSocketEvent
            {
                CorrelationId = correlationId,
                SocketIds = [notification.ReceiverId.ToString()],
                NotificationId = notification.Id,
                SubjectId = subject.Id,
                SubjectUserName = subject.UserName,
                SubjectAvatar = subject.Avatar,
                SubjectIsOfficial = subject.IsOfficial,
                NotificationType = (int)notification.Type,
                DirectObjectId = notification.DirectObjectId,
                IndirectObjectId = notification.IndirectObjectId,
                PrepositionalObjectId = notification.PrepositionalObjectId,
                CreatedAt = notification.CreatedAt
            });
        }
    }
}