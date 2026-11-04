using System.Net;
using System.Reflection;
using AuroraScienceHub.Integrations.WdcClient;
using AuroraScienceHub.Integrations.WdcClient.DstIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.WdcClient;

public sealed class ServiceCollectionExtensionsTests
{
    [Theory(DisplayName = "WDC registration binds options, resolves the client and configures the proxy")]
    [InlineData(false)]
    [InlineData(true)]
    public void AddWdcClients_WhenConfigured_ResolvesClientAndProxy(bool useProxy)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Wdc:ServerUrl"] = "https://wdc.test/",
            ["Wdc:UseProxy"] = useProxy.ToString(),
            ["Proxy:Address"] = "http://proxy.test:8080",
            ["Proxy:UserName"] = "user",
            ["Proxy:Password"] = "password"
        }).Build();
        var services = new ServiceCollection();

        // Act
        services.AddWdcClients(configuration);
        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IDstIndexClient>();
        var options = provider.GetRequiredService<IOptions<WdcClientOptions>>().Value;
        var handler = provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(nameof(IDstIndexClient));
        var primaryHandler = UnwrapPrimaryHandler(handler);

        // Assert
        client.ShouldNotBeNull();
        options.RequiredServerUrl.ShouldBe(new Uri("https://wdc.test/"));
        options.UseProxy.ShouldBe(useProxy);
        var proxy = primaryHandler switch
        {
            HttpClientHandler httpClientHandler => httpClientHandler.Proxy,
            SocketsHttpHandler socketsHttpHandler => socketsHttpHandler.Proxy,
            _ => throw new InvalidOperationException($"Unexpected primary handler type {primaryHandler.GetType().Name}.")
        };
        var address = proxy?.GetProxy(new Uri("https://wdc.test/"));
        address.ShouldBe(useProxy ? new Uri("http://proxy.test:8080") : null);
        var credentials = proxy?.Credentials?.GetCredential(new Uri("http://proxy.test:8080"), "Basic");
        var userName = credentials?.UserName;
        var password = credentials?.Password;
        userName.ShouldBe(useProxy ? "user" : null);
        password.ShouldBe(useProxy ? "password" : null);
    }

    [Fact(DisplayName = "WDC proxy configuration fails fast when the proxy address is missing")]
    public void AddWdcClients_WhenProxyAddressMissing_Throws()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Wdc:ServerUrl"] = "https://wdc.test/",
            ["Wdc:UseProxy"] = "true"
        }).Build();
        var services = new ServiceCollection();
        services.AddWdcClients(configuration);
        using var provider = services.BuildServiceProvider();

        // Act
        var exception = Should.Throw<InvalidOperationException>(() => provider.GetRequiredService<IDstIndexClient>());

        // Assert
        exception.Message.ShouldContain("Proxy");
    }

    [Fact(DisplayName = "WDC options report a missing server URL")]
    public void RequiredServerUrl_WhenMissing_ThrowsDescriptiveException()
    {
        // Arrange
        var options = new WdcClientOptions();

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => options.RequiredServerUrl);

        // Assert
        exception.Message.ShouldContain("Wdc:ServerUrl");
    }

    private static HttpMessageHandler UnwrapPrimaryHandler(HttpMessageHandler handler)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        while (handler is not HttpClientHandler and not SocketsHttpHandler)
        {
            var innerHandlerProperty = handler.GetType().GetProperty("InnerHandler", flags)
                ?? throw new InvalidOperationException($"Cannot unwrap message handler of type {handler.GetType().Name}.");
            handler = innerHandlerProperty.GetValue(handler) as HttpMessageHandler
                ?? throw new InvalidOperationException($"Inner handler is missing on {handler.GetType().Name}.");
        }

        return handler;
    }

}
