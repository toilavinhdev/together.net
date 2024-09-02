using Microsoft.EntityFrameworkCore;

namespace Service.Notification.Domain;

public static class NotificationContextInitialization
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<NotificationContext>>();
        
        logger.LogInformation("Starting to migrate database");
        
        await context.Database.MigrateAsync();

        await context.Database.EnsureCreatedAsync();
        
        await context.SaveChangesAsync();
        
        logger.LogInformation("Database migration completed");
    }
}