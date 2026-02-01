using AuroraScienceHub.Integrations.Noaa.Ace;
using AuroraScienceHub.Integrations.Noaa.Dscovr;
using AuroraScienceHub.Integrations.Noaa.KpIndex;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Integrations.Noaa;

/// <summary>
/// <see cref="IServiceCollection"/> to register NOAA clients.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds NOAA clients to the service collection.
    /// </summary>
    public static IServiceCollection AddNoaaClients(this IServiceCollection services)
    {
        services.AddOptions<NoaaClientOptions>()
            .BindConfiguration(NoaaClientOptions.OptionKey);

        services.AddHttpClient<IAceClient, AceClient>();
        services.AddHttpClient<IDscovrClient, DscovrClient>();
        services.AddHttpClient<IKpIndexClient, KpIndexClient>();

        return services;
    }
}
