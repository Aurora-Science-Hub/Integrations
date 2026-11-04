using AuroraScienceHub.Framework.Http.Proxy;
using AuroraScienceHub.Integrations.KyotoClient.DstIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Integrations.KyotoClient;

/// <summary>
/// Registration of Kyoto clients
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Kyoto clients and binds the Kyoto and Proxy configuration sections
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddKyotoClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<KyotoClientOptions>()
            .Bind(configuration.GetSection(KyotoClientOptions.OptionKey));
        services.AddOptions<ProxyOptions>()
            .Bind(configuration.GetSection(ProxyOptions.OptionKey));

        var builder = services.AddHttpClient<IDstIndexClient, DstIndexClient>();
        if (configuration.GetSection(KyotoClientOptions.OptionKey).GetValue<bool>(nameof(KyotoClientOptions.UseProxy)))
        {
            builder.ConfigurePrimaryHttpProxyMessageHandler();
        }

        return services;
    }
}
