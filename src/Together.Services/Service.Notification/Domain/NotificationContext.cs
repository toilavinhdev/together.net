using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Service.Notification.Domain;
using Notification = Aggregates.NotificationAggregate.Notification;

public sealed class NotificationContext(DbContextOptions<NotificationContext> options, PostgresConfig config) 
    : PostgresDbContext<NotificationContext>(options, config)
{
    public DbSet<Notification> Notifications { get; set; }
}