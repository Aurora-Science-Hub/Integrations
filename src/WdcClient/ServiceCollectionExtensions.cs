using AuroraScienceHub.Framework.Http.Proxy;
using AuroraScienceHub.Integrations.WdcClient.DstIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Integrations.WdcClient;

/// <summary>
/// Registration of World Data Center for Geomagnetism clients
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers World Data Center for Geomagnetism clients and binds the Wdc and Proxy configuration sections
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddWdcClients(this IServiceCollection services, IConfiguration configuration)
    {
        var wdcSection = configuration.GetSection(WdcClientOptions.OptionKey);
        services.AddOptions<WdcClientOptions>()
            .Bind(wdcSection);
        services.AddOptions<ProxyOptions>()
            .Bind(configuration.GetSection(ProxyOptions.OptionKey));

        var useProxy = wdcSection.GetValue<bool>(nameof(WdcClientOptions.UseProxy));
        var builder = services.AddHttpClient<IDstIndexClient, DstIndexClient>();
        if (useProxy)
        {
            builder.ConfigurePrimaryHttpProxyMessageHandler();
        }

        return services;
    }
}
