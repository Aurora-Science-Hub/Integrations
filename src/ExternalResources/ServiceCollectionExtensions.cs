using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Ace;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Dscovr;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Infrastructure.Configuration;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.KpIndex;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAceClient(this IServiceCollection services)
    {
        services.AddOptions<NoaaClientOptions>()
            .BindConfiguration(NoaaClientOptions.OptionKey);
        services.AddOptions<AariGeophysClientOptions>()
            .BindConfiguration(AariGeophysClientOptions.OptionKey);

        services.AddHttpClient<IAceClient, AceClient>();
        services.AddHttpClient<IDscovrClient, DscovrClient>();
        services.AddHttpClient<IKpIndexClient, KpIndexClient>();
        services.AddHttpClient<IPcIndexClient, PcIndexClient>();

        return services;
    }
}
