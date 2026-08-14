using AuroraScienceHub.Framework.Http.Proxy;
using AuroraScienceHub.Integrations.NoaaClient.Ace;
using AuroraScienceHub.Integrations.NoaaClient.KpIndex;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw;
using AuroraScienceHub.Integrations.NoaaClient.WsaEnlil;
using Microsoft.Extensions.Configuration;
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
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application configuration used to read the <see cref="NoaaClientOptions"/> section.</param>
    /// <remarks>
    /// Registers <see cref="Ace.IAceClient"/> for backward compatibility only; prefer <see cref="Rtsw.IRtswClient"/>.
    /// <para>
    /// When <see cref="NoaaClientOptions.UseProxy"/> is enabled in configuration, client requests are routed
    /// through the proxy configured in the <c>Proxy</c> section (see <see cref="ProxyOptions"/>).
    /// </para>
    /// </remarks>
    public static IServiceCollection AddNoaaClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<NoaaClientOptions>()
            .Bind(configuration.GetSection(NoaaClientOptions.OptionKey));

        services.AddOptions<ProxyOptions>()
            .Bind(configuration.GetSection(ProxyOptions.OptionKey));

        var useProxy = configuration
            .GetSection(NoaaClientOptions.OptionKey)
            .GetValue<bool>(nameof(NoaaClientOptions.UseProxy));

#pragma warning disable CS0618 // ACE client registration pending removal in issue #3.
        services.AddNoaaHttpClient<IAceClient, AceClient>(useProxy);
#pragma warning restore CS0618
        services.AddNoaaHttpClient<IKpIndexClient, KpIndexClient>(useProxy);
        services.AddNoaaHttpClient<IRtswClient, RtswClient>(useProxy);
        services.AddNoaaHttpClient<IWsaEnlilClient, WsaEnlilClient>(useProxy);

        return services;
    }

    private static void AddNoaaHttpClient<TClient, TImplementation>(
        this IServiceCollection services,
        bool useProxy)
        where TClient : class
        where TImplementation : class, TClient
    {
        var builder = services.AddHttpClient<TClient, TImplementation>();

        if (useProxy)
        {
            builder.ConfigurePrimaryHttpProxyMessageHandler();
        }
    }
}
