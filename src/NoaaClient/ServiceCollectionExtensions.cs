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
    /// <remarks>
    /// Registers <see cref="Ace.IAceClient"/> for backward compatibility only; prefer <see cref="Rtsw.IRtswClient"/>.
    /// </remarks>
    public static IServiceCollection AddNoaaClients(this IServiceCollection services)
    {
        services.AddOptions<NoaaClientOptions>()
            .BindConfiguration(NoaaClientOptions.OptionKey);

#pragma warning disable CS0618 // ACE client registration pending removal in issue #3.
        services.AddHttpClient<IAceClient, AceClient>();
#pragma warning restore CS0618
        services.AddHttpClient<IKpIndexClient, KpIndexClient>();
        services.AddHttpClient<IRtswClient, RtswClient>();

        return services;
    }
}
