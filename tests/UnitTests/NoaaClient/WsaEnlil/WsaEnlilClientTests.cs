using System.Net;
using AuroraScienceHub.Integrations.NoaaClient;
using AuroraScienceHub.Integrations.NoaaClient.WsaEnlil;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.WsaEnlil;

/// <summary>
/// Unit tests for <see cref="AuroraScienceHub.Integrations.NoaaClient.WsaEnlil.WsaEnlilClient"/>.
/// </summary>
/// <remarks>
/// Tests cover the HTTP/download pipeline. ffmpeg encoding is an integration concern
/// and is not covered by unit tests.
/// </remarks>
public sealed class WsaEnlilClientTests
{
    private static readonly Uri BaseUrl = new("https://noaa.test");

    [Fact(DisplayName = "Returns empty stream when NOAA manifest is an empty JSON array")]
    public async Task GetEnlilAnimationAsync_WhenManifestIsEmpty_ReturnsEmptyStream()
    {
        // Arrange
        var sut = CreateSut("[]");

        // Act
        await using var result = await sut.GetEnlilAnimationAsync(
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Length.ShouldBe(0);
    }

    [Fact(DisplayName = "Returns empty stream when NOAA manifest deserializes to null")]
    public async Task GetEnlilAnimationAsync_WhenManifestIsNull_ReturnsEmptyStream()
    {
        // Arrange
        var sut = CreateSut("null");

        // Act
        await using var result = await sut.GetEnlilAnimationAsync(
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.Length.ShouldBe(0);
    }

    [Theory(DisplayName = "Throws ArgumentOutOfRangeException when maxWidth is zero or negative")]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetEnlilAnimationAsync_WhenMaxWidthIsInvalid_ThrowsArgumentOutOfRangeException(int maxWidth)
    {
        // Arrange
        var sut = CreateSut("[]");

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            async () => await sut.GetEnlilAnimationAsync(
                maxWidth: maxWidth,
                cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact(DisplayName = "Throws OperationCanceledException when token is pre-cancelled")]
    public async Task GetEnlilAnimationAsync_WhenTokenIsCancelled_ThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var sut = CreateSut("[]");

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            async () => await sut.GetEnlilAnimationAsync(cancellationToken: cts.Token));
    }

    [Fact(DisplayName = "Throws when token is cancelled mid-download")]
    public async Task GetEnlilAnimationAsync_WhenCancelledDuringDownload_ThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        var manifestJson = """
        [
            {"url":"/images/animations/enlil/frame1.jpg"},
            {"url":"/images/animations/enlil/frame2.jpg"},
            {"url":"/images/animations/enlil/frame3.jpg"}
        ]
        """;

        var handler = new CancellingHandler(manifestJson, cancelAfterRequests: 2, cts);
        using var httpClient = new HttpClient(handler);
        var options = Options.Create(new NoaaClientOptions { ServerUrl = BaseUrl });
        var sut = new WsaEnlilClient(httpClient, options);

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            async () => await sut.GetEnlilAnimationAsync(maxWidth: 420, cancellationToken: cts.Token));
    }

    private static IWsaEnlilClient CreateSut(string manifestJson)
    {
        var handler = new TestHttpMessageHandler(CreateJsonResponse(manifestJson));
        var httpClient = new HttpClient(handler);
        var options = Options.Create(new NoaaClientOptions { ServerUrl = BaseUrl });
        return new WsaEnlilClient(httpClient, options);
    }

    private static HttpResponseMessage CreateJsonResponse(string json)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
    }

    /// <summary>
    /// Simple test double that returns pre-configured responses.
    /// </summary>
    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses;

        public TestHttpMessageHandler(params HttpResponseMessage[] responses)
        {
            _responses = new Queue<HttpResponseMessage>(responses);
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_responses.Count > 0
                ? _responses.Dequeue()
                : new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }

    /// <summary>
    /// Handler that cancels the token after a configured number of requests.
    /// </summary>
    private sealed class CancellingHandler : HttpMessageHandler
    {
        private readonly string _manifestJson;
        private readonly int _cancelAfterRequests;
        private readonly CancellationTokenSource _cts;
        private int _requestCount;

        public CancellingHandler(string manifestJson, int cancelAfterRequests, CancellationTokenSource cts)
        {
            _manifestJson = manifestJson;
            _cancelAfterRequests = cancelAfterRequests;
            _cts = cts;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var count = Interlocked.Increment(ref _requestCount);
            if (count > _cancelAfterRequests)
            {
                await _cts.CancelAsync();
                // Give the cancellation a moment to propagate
                await Task.Delay(50, CancellationToken.None);
                cancellationToken.ThrowIfCancellationRequested();
            }

            // Simulate network latency so cancellation can be observed
            await Task.Delay(100, CancellationToken.None);

            return count == 1
                ? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_manifestJson, System.Text.Encoding.UTF8, "application/json")
                }
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent("fake-jpeg-data"u8.ToArray())
                };
        }
    }
}
