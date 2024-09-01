using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Cloudinary;

public static class CloudinaryExtensions
{
    public static void AddCloudinary(this IServiceCollection services, string url)
    {
        services.AddTransient<ICloudinaryService>(_ => new CloudinaryService(url));
    }
}