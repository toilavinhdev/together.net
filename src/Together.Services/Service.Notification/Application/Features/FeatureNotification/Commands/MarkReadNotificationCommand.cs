using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Notification.Domain;
using Service.Notification.Domain.Enums;

namespace Service.Notification.Application.Features.FeatureNotification.Commands;

public sealed class MarkReadNotificationCommand : IBaseRequest
{
    public Guid Id { get; set; }
    
    public class Validator : AbstractValidator<MarkReadNotificationCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, NotificationContext context) 
        : BaseRequestHandler<MarkReadNotificationCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(MarkReadNotificationCommand request, CancellationToken ct)
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == request.Id, ct);

            if (notification is null) throw new TogetherException(ErrorCodes.Notification.NotificationNotFound);

            notification.Status = NotificationStatus.Read;

            context.Notifications.Update(notification);

            await context.SaveChangesAsync(ct);
        }
    }
}