using System.Net;
using AuroraScienceHub.Integrations.FmiClient;
using AuroraScienceHub.Integrations.FmiClient.FmiElectrojet;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.FmiClient.FmiElectrojet;

public sealed class FmiElectrojetClientTests
{
    [Fact(DisplayName = "FMI electrojet request retrieves IU and IL together from the real-time file")]
    public async Task GetIndicesAsync_WhenSuccess_RequestsFileAndReturnsBothIndices()
    {
        // Arrange
        const string text = "2026 11 04 15 15 00 -4.2 -8.0 10\n2026 11 04 15 16 00 -4.3 -7.2 9";
        using var handler = new TestHandler(Response(HttpStatusCode.OK, text));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var result = await sut.GetIndicesAsync(TestContext.Current.CancellationToken);

        // Assert
        handler.Requests.ShouldBe([new Uri("https://fmi.test/image/realtime/eurisgic/realtime_iu_il.txt")]);
        result.Count.ShouldBe(2);
        result[0].Iu.ShouldBe(-4.2f);
        result[0].Il.ShouldBe(-8.0f);
        result[0].StationCount.ShouldBe(10);
        result[1].Iu.ShouldBe(-4.3f);
        result[1].Il.ShouldBe(-7.2f);
    }

    [Fact(DisplayName = "FMI electrojet returns no records for an empty response")]
    public async Task GetIndicesAsync_WhenEmptyBody_ReturnsEmpty()
    {
        // Arrange
        using var handler = new TestHandler(Response(HttpStatusCode.OK, string.Empty));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var result = await sut.GetIndicesAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeEmpty();
    }

    [Theory(DisplayName = "FMI electrojet throws for missing files and HTTP errors")]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetIndicesAsync_WhenHttpError_Throws(HttpStatusCode status)
    {
        // Arrange
        using var handler = new TestHandler(Response(status, "error"));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var exception = await Should.ThrowAsync<HttpRequestException>(
            () => sut.GetIndicesAsync(TestContext.Current.CancellationToken));

        // Assert
        exception.StatusCode.ShouldBe(status);
    }

    private static IFmiElectrojetClient CreateSut(HttpClient http)
        => new FmiElectrojetClient(http, Options.Create(new FmiClientOptions { ServerUrl = new Uri("https://fmi.test/") }));

    private static HttpResponseMessage Response(HttpStatusCode status, string content)
        => new(status) { Content = new StringContent(content) };

    private sealed class TestHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add(request.RequestUri ?? throw new InvalidOperationException("Request URI is missing."));
            return Task.FromResult(response);
        }
    }
}
