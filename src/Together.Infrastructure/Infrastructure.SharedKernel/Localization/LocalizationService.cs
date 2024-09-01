using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.SharedKernel.Localization;

public interface ILocalizationService
{
    IEnumerable<LocalizedString> Keys();
    
    string Get(string name);
}

public class LocalizationService(IStringLocalizerFactory factory) : ILocalizationService
{
    private readonly IStringLocalizer _localize = factory.Create(typeof(SharedResource));
    
    public IEnumerable<LocalizedString> Keys() => _localize.GetAllStrings();
    
    public string Get(string name) => _localize[name];
}

public static class LocalizationServiceFactory
{
    private static readonly Lazy<LocalizationService> LocalizationServiceInstance =
        new(() =>
        {
            var factory = new ResourceManagerStringLocalizerFactory(
                new OptionsWrapper<LocalizationOptions>(new LocalizationOptions()),
                new LoggerFactory()
            );

            return new LocalizationService(factory);
        });

    public static LocalizationService GetInstance() => LocalizationServiceInstance.Value;
}