using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Events;
using Infrastructure.SharedKernel.Enums;
using MediatR;
using Service.Notification.Application.Features.FeatureNotification.Commands;

namespace Service.Notification.BackgroundServices;

public sealed class CreateNotificationBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IRabbitMQClient _eventBus = serviceProvider.GetRequiredService<IRabbitMQClient>();

    private readonly ISender _sender = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ISender>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _eventBus.Subscribe<CreateNotificationEvent>(async @event =>
        {
            await _sender.Send(new CreateNotificationCommand
            {
                ReceiverId = @event.ReceiverId,
                SubjectId = @event.SubjectId,
                Type = (NotificationType)@event.NotificationType,
                DirectObjectId = @event.DirectObjectId,
                IndirectObjectId = @event.IndirectObjectId,
                PrepositionalObjectId = @event.PrepositionalObjectId
            }, stoppingToken);
        });

        await Task.CompletedTask;
    }
}