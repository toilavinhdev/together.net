using Microsoft.EntityFrameworkCore;
using Service.Community.Domain.Aggregates.ForumAggregate;
using Service.Community.Domain.Aggregates.PrefixAggregate;
using Service.Community.Domain.Aggregates.TopicAggregate;

namespace Service.Community.Domain;

public static class CommunityContextInitialization
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CommunityContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<CommunityContext>>();
        
        await context.Database.MigrateAsync();
        await context.Database.EnsureCreatedAsync();
        logger.LogInformation("Starting to migrate database");
        if (!await context.Forums.AnyAsync()) await context.Forums.AddRangeAsync(Forums);
        if (!await context.Prefixes.AnyAsync()) await context.Prefixes.AddRangeAsync(Prefixes);
        await context.SaveChangesAsync();
        logger.LogInformation("Database migration completed");
    }

    private static readonly List<Forum> Forums =
    [
        new Forum
        {
            Id = Guid.NewGuid(),
            Name = "Khu vực điều hành",
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow,
            CreatedById = Guid.Empty,
            ModifiedById = Guid.Empty,
            Topics = [
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "Thông Báo Từ BQT",
                    Description = "Các thông báo mới từ ban quản trị",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ModifiedAt = DateTimeOffset.UtcNow,
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                }
            ]
        },
        new Forum
        {
            Id = Guid.NewGuid(),
            Name = "Học tập & Sự nghiệp",
            CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1),
            ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(1),
            CreatedById = Guid.Empty,
            ModifiedById = Guid.Empty,
            Topics = [
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "Tuyển dụng & Tìm việc",
                    Description = "Nơi đăng tin tuyển dụng và tìm việc",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ModifiedAt = DateTimeOffset.UtcNow,
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                },
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "Ngoại ngữ",
                    Description = "Góc ngoại ngữ",
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                },
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "Lập trình - IT",
                    Description = "Fix bugs never give up",
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(2),
                    ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(2),
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                },
            ]
        },
        new Forum
        {
            Id = Guid.NewGuid(),
            Name = "Sản phẩm công nghệ",
            CreatedAt = DateTimeOffset.UtcNow.AddSeconds(2),
            ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(2),
            CreatedById = Guid.Empty,
            ModifiedById = Guid.Empty,
            Topics = [
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "Chụp ảnh & Quay phim",
                    Description = "Giao lưu & Chia sẻ kinh nghiệm",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ModifiedAt = DateTimeOffset.UtcNow,
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                },
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "Android",
                    Description = "Hội những người yêu thích Android",
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                },
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "IOS",
                    Description = "Hội những người dùng IOS",
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(2),
                    ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(2),
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                }
            ]
        },
        new Forum
        {
            Id = Guid.NewGuid(),
            Name = "Khác",
            CreatedAt = DateTimeOffset.UtcNow.AddSeconds(3),
            ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(3),
            CreatedById = Guid.Empty,
            ModifiedById = Guid.Empty,
            Topics = [
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "Log bugs",
                    Description = "Nơi tiếp nhận & xử lý bugs của Together.NET",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ModifiedAt = DateTimeOffset.UtcNow,
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                },
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = "Nhận xét sản phẩm",
                    Description = "Tất cả nhận xét về project Together.NET",
                    CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(1),
                    CreatedById = Guid.Empty,
                    ModifiedById = Guid.Empty
                }
            ]
        }
    ];

    private static readonly List<Prefix> Prefixes =
    [
        new Prefix
        {
            Id = Guid.NewGuid(),
            Name = "Thông báo",
            Foreground = "#FFFFFF",
            Background = "#E20000",
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow,
            CreatedById = Guid.Empty,
            ModifiedById = Guid.Empty
        },
        new Prefix
        {
            Id = Guid.NewGuid(),
            Name = "Kiến thức",
            Foreground = "#FFFFFF",
            Background = "#008000",
            CreatedAt = DateTimeOffset.UtcNow.AddSeconds(1),
            ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(1),
            CreatedById = Guid.Empty,
            ModifiedById = Guid.Empty
        },
        new Prefix
        {
            Id = Guid.NewGuid(),
            Name = "Thảo luận",
            Foreground = "#000000",
            Background = "#FFCB00",
            CreatedAt = DateTimeOffset.UtcNow.AddSeconds(2),
            ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(2),
            CreatedById = Guid.Empty,
            ModifiedById = Guid.Empty
        },
        new Prefix
        {
            Id = Guid.NewGuid(),
            Name = "Chia sẻ",
            Foreground = "#FFFFFF",
            Background = "#4338CA",
            CreatedAt = DateTimeOffset.UtcNow.AddSeconds(3),
            ModifiedAt = DateTimeOffset.UtcNow.AddSeconds(3),
            CreatedById = Guid.Empty,
            ModifiedById = Guid.Empty
        }
    ];
}