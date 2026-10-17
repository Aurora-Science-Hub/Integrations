using System.Net;
using System.Reflection;
using AuroraScienceHub.Integrations.DtuSpaceClient;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.DtuSpaceClient;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/> proxy configuration.
/// </summary>
public sealed class ServiceCollectionExtensionsTests
{
    private static readonly Uri ProxyAddress = new("http://proxy.example.com:8080");

    [Fact(DisplayName = "DTU Space clients do not use a custom proxy when UseProxy is not enabled")]
    public void AddDtuSpaceClients_WhenUseProxyDisabled_ConfiguresClientWithoutProxy()
    {
        // Arrange / Act
        var (_, proxy) = CreatePrimaryHandlerProxy(CreateConfiguration(useProxy: false), typeof(IPcIndexClient));

        // Assert
        proxy.ShouldBeNull();
    }

    [Fact(DisplayName = "DTU Space clients route through the configured proxy when UseProxy is enabled")]
    public void AddDtuSpaceClients_WhenUseProxyEnabled_ConfiguresProxyAddress()
    {
        // Arrange / Act
        var (useProxy, proxy) = CreatePrimaryHandlerProxy(
            CreateConfiguration(useProxy: true, address: ProxyAddress),
            typeof(IPcIndexClient));

        // Assert
        useProxy.ShouldBeTrue();
        var webProxy = proxy.ShouldBeOfType<WebProxy>();
        webProxy.Address.ShouldBe(ProxyAddress);
    }

    [Fact(DisplayName = "DTU Space clients apply proxy credentials when configured")]
    public void AddDtuSpaceClients_WhenProxyCredentialsConfigured_AppliesCredentials()
    {
        // Arrange / Act
        var (_, proxy) = CreatePrimaryHandlerProxy(
            CreateConfiguration(useProxy: true, address: ProxyAddress, userName: "user", password: "pass"),
            typeof(IPcIndexClient));

        // Assert
        var webProxy = proxy.ShouldBeOfType<WebProxy>();
        var credentials = webProxy.Credentials.ShouldBeOfType<NetworkCredential>();
        credentials.UserName.ShouldBe("user");
        credentials.Password.ShouldBe("pass");
        webProxy.UseDefaultCredentials.ShouldBeFalse();
    }

    [Fact(DisplayName = "DTU Space clients fail fast when UseProxy is enabled but the proxy address is missing")]
    public void AddDtuSpaceClients_WhenUseProxyEnabledWithoutAddress_ThrowsInvalidOperationException()
    {
        // Arrange / Act
        var exception = Should.Throw<InvalidOperationException>(
            () => CreatePrimaryHandlerProxy(CreateConfiguration(useProxy: true), typeof(IPcIndexClient)));

        // Assert
        exception.Message.ShouldContain("Proxy");
    }

    private static (bool UseProxy, IWebProxy? Proxy) CreatePrimaryHandlerProxy(
        IConfiguration configuration,
        Type clientType)
    {
        var services = new ServiceCollection();
        services.AddDtuSpaceClients(configuration);

        using var provider = services.BuildServiceProvider();

        var handlerFactory = provider.GetRequiredService<IHttpMessageHandlerFactory>();
        var handler = handlerFactory.CreateHandler(clientType.Name);
        var primaryHandler = UnwrapPrimaryHandler(handler);

        return primaryHandler switch
        {
            HttpClientHandler httpClientHandler => (httpClientHandler.UseProxy, httpClientHandler.Proxy),
            SocketsHttpHandler socketsHandler => (socketsHandler.UseProxy, socketsHandler.Proxy),
            _ => throw new InvalidOperationException(
                $"Unexpected primary handler type {primaryHandler.GetType().Name}."),
        };
    }

    private static HttpMessageHandler UnwrapPrimaryHandler(HttpMessageHandler handler)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        while (true)
        {
            if (handler is HttpClientHandler or SocketsHttpHandler)
            {
                return handler;
            }

            var innerHandlerProperty = handler.GetType().GetProperty("InnerHandler", flags)
                                       ?? throw new InvalidOperationException(
                                           $"Cannot unwrap message handler of type {handler.GetType().Name}.");

            handler = (HttpMessageHandler)innerHandlerProperty.GetValue(handler)!;
        }
    }

    private static IConfiguration CreateConfiguration(
        bool useProxy,
        Uri? address = null,
        string? userName = null,
        string? password = null)
    {
        var data = new Dictionary<string, string?>
        {
            [$"{DtuSpaceClientOptions.OptionKey}:ServerUrl"] = "https://dtu.test",
            [$"{DtuSpaceClientOptions.OptionKey}:UseProxy"] = useProxy.ToString(),
        };

        if (address is not null)
        {
            data["Proxy:Address"] = address.ToString();
        }

        if (userName is not null)
        {
            data["Proxy:UserName"] = userName;
        }

        if (password is not null)
        {
            data["Proxy:Password"] = password;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();
    }
}
