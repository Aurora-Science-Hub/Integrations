using AuroraScienceHub.Framework.Http.Proxy;
using AuroraScienceHub.Integrations.WdcClient.DstIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Integrations.WdcClient;

/// <summary>
/// Registration of Kyoto clients
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Kyoto clients and binds the Wdc and Proxy configuration sections
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddWdcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<WdcClientOptions>()
            .Bind(configuration.GetSection(WdcClientOptions.OptionKey));
        services.AddOptions<ProxyOptions>()
            .Bind(configuration.GetSection(ProxyOptions.OptionKey));

        var builder = services.AddHttpClient<IDstIndexClient, DstIndexClient>();
        if (configuration.GetSection(WdcClientOptions.OptionKey).GetValue<bool>(nameof(WdcClientOptions.UseProxy)))
        {
            builder.ConfigurePrimaryHttpProxyMessageHandler();
        }

        return services;
    }
}
