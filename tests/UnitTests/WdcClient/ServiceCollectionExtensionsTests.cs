using System.Net;
using AuroraScienceHub.Integrations.WdcClient;
using AuroraScienceHub.Integrations.WdcClient.DstIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.WdcClient;

public sealed class ServiceCollectionExtensionsTests
{
    [Theory(DisplayName = "Kyoto registration binds options, resolves the client and configures the proxy")]
    [InlineData(false)]
    [InlineData(true)]
    public void AddWdcClients_WhenConfigured_ResolvesClientAndProxy(bool useProxy)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Wdc:ServerUrl"] = "https://kyoto.test/",
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
        while (handler is DelegatingHandler delegating)
        {
            handler = delegating.InnerHandler ?? throw new InvalidOperationException("Inner handler is missing.");
        }

        // Assert
        client.ShouldNotBeNull();
        options.RequiredServerUrl.ShouldBe(new Uri("https://kyoto.test/"));
        options.UseProxy.ShouldBe(useProxy);
        var proxy = (handler as HttpClientHandler)?.Proxy ?? (handler as SocketsHttpHandler)?.Proxy;
        var address = proxy?.GetProxy(new Uri("https://kyoto.test/"));
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
            ["Wdc:ServerUrl"] = "https://kyoto.test/",
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

    [Fact(DisplayName = "Kyoto options report a missing server URL")]
    public void RequiredServerUrl_WhenMissing_ThrowsDescriptiveException()
    {
        // Arrange
        var options = new WdcClientOptions();

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => options.RequiredServerUrl);

        // Assert
        exception.Message.ShouldContain("Wdc:ServerUrl");
    }
}
