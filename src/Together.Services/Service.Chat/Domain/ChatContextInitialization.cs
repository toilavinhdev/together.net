using Microsoft.EntityFrameworkCore;

namespace Service.Chat.Domain;

public static class ChatContextInitialization
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<ChatContext>>();
        
        await context.Database.MigrateAsync();

        await context.Database.EnsureCreatedAsync();
        
        logger.LogInformation("Starting to migrate database");

        await context.SaveChangesAsync();
        
        logger.LogInformation("Database migration completed");
    }
}