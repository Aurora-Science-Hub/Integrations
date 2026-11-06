using System.Diagnostics.CodeAnalysis;
using AuroraScienceHub.Framework.Http.Proxy;
using AuroraScienceHub.Integrations.FmiClient.FmiElectrojet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Integrations.FmiClient;

/// <summary>
/// Registration of Finnish Meteorological Institute clients
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Finnish Meteorological Institute clients and binds the Fmi and Proxy configuration sections
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddFmiClients(this IServiceCollection services, IConfiguration configuration)
    {
        var fmiSection = configuration.GetSection(FmiClientOptions.OptionKey);
        services.AddOptions<FmiClientOptions>()
            .Bind(fmiSection);
        services.AddOptions<ProxyOptions>()
            .Bind(configuration.GetSection(ProxyOptions.OptionKey));

        var useProxy = fmiSection.GetValue<bool>(nameof(FmiClientOptions.UseProxy));
        services.AddFmiHttpClient<IFmiElectrojetClient, FmiElectrojetClient>(useProxy);

        return services;
    }

    private static void AddFmiHttpClient<TClient, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
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
