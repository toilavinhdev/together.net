using System.Globalization;
using Infrastructure.SharedKernel.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.SharedKernel.Extensions;

public static class LanguageExtensions
{
    private static readonly List<CultureInfo> SupportedCultures =
    [
        new("en-US"),
        new("vi-VN")
    ];
    
    public static void AddCoreLanguages(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddSingleton<ILocalizationService, LocalizationService>();
    }

    public static void UseLanguages(this WebApplication app)
    {
        app.UseRequestLocalization(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(SupportedCultures[0]);
            options.SupportedCultures = SupportedCultures;
            options.SupportedUICultures = SupportedCultures;
            options.RequestCultureProviders =
            [
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
            options.ApplyCurrentCultureToResponseHeaders = true;
        });
    }
}