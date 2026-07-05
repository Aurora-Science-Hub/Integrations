using AuroraScienceHub.Integrations.NoaaClient.Ace;
using AuroraScienceHub.Integrations.NoaaClient.KpIndex;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Integrations.NoaaClient;

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
        services.AddHttpClient<IKpIndexClient, KpIndexClient>();
        services.AddHttpClient<IRtswClient, RtswClient>();

        return services;
    }
}
