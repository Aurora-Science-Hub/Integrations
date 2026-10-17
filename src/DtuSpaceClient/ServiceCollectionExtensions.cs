using System.Diagnostics.CodeAnalysis;
using AuroraScienceHub.Framework.Http.Proxy;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Integrations.DtuSpaceClient;

/// <summary>
/// <see cref="IServiceCollection"/> to register DTU Space clients.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds DTU Space clients to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application configuration used to read the <see cref="DtuSpaceClientOptions"/> section.</param>
    /// <remarks>
    /// When <see cref="DtuSpaceClientOptions.UseProxy"/> is enabled in configuration, client requests are routed
    /// through the proxy configured in the <c>Proxy</c> section (see <see cref="ProxyOptions"/>).
    /// </remarks>
    public static IServiceCollection AddDtuSpaceClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DtuSpaceClientOptions>()
            .Bind(configuration.GetSection(DtuSpaceClientOptions.OptionKey));

        services.AddOptions<ProxyOptions>()
            .Bind(configuration.GetSection(ProxyOptions.OptionKey));

        var useProxy = configuration
            .GetSection(DtuSpaceClientOptions.OptionKey)
            .GetValue<bool>(nameof(DtuSpaceClientOptions.UseProxy));

        services.AddDtuSpaceHttpClient<IPcIndexClient, PcIndexClient>(useProxy);

        return services;
    }

    private static void AddDtuSpaceHttpClient<TClient, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
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
