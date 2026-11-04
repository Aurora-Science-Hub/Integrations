using System.Net;
using AuroraScienceHub.Integrations.KyotoClient;
using AuroraScienceHub.Integrations.KyotoClient.DstIndex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.KyotoClient;

public sealed class ServiceCollectionExtensionsTests
{
    [Theory(DisplayName = "Kyoto registration binds options, resolves the client and configures the proxy")]
    [InlineData(false)]
    [InlineData(true)]
    public void AddKyotoClients_WhenConfigured_ResolvesClientAndProxy(bool useProxy)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Kyoto:ServerUrl"] = "https://kyoto.test/",
            ["Kyoto:UseProxy"] = useProxy.ToString(),
            ["Proxy:Address"] = "http://proxy.test:8080",
            ["Proxy:UserName"] = "user",
            ["Proxy:Password"] = "password"
        }).Build();
        var services = new ServiceCollection();

        // Act
        services.AddKyotoClients(configuration);
        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IDstIndexClient>();
        var options = provider.GetRequiredService<IOptions<KyotoClientOptions>>().Value;
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

    [Fact(DisplayName = "Kyoto options report a missing server URL")]
    public void RequiredServerUrl_WhenMissing_ThrowsDescriptiveException()
    {
        // Arrange
        var options = new KyotoClientOptions();

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => options.RequiredServerUrl);

        // Assert
        exception.Message.ShouldContain("Kyoto:ServerUrl");
    }
}
