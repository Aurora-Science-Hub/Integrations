using System.Net;
using AuroraScienceHub.Integrations.WdcClient;
using AuroraScienceHub.Integrations.WdcClient.DstIndex;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.WdcClient.DstIndex;

public sealed class DstIndexClientTests
{
    [Fact(DisplayName = "Dst day request uses one monthly file and filters other days")]
    public async Task GetDstAsync_WhenDay_RequestsMonthlyFileAndFiltersDay()
    {
        // Arrange
        using var handler = new TestHandler(_ => Response(HttpStatusCode.OK, Day("2611", "01") + "\r\n" + Day("2611", "02")));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var result = await sut.GetDstAsync(new DateOnly(2026, 11, 2), TestContext.Current.CancellationToken);

        // Assert
        handler.Requests.ShouldBe([new Uri("https://wdc.test/dst_realtime/202611/dst2611.for.request")]);
        result.Count.ShouldBe(24);
        result.ShouldAllBe(record => record.DateTime.Day == 2 && record.Value == -1);
    }

    [Fact(DisplayName = "Dst range requests each month once across a year boundary")]
    public async Task GetDstAsync_WhenYearBoundary_ReturnsInclusiveDaysInOrder()
    {
        // Arrange
        using var handler = new TestHandler(uri => Response(HttpStatusCode.OK,
            uri.AbsolutePath.Contains("202612", StringComparison.Ordinal)
                ? Day("2612", "30") + "\n" + Day("2612", "31")
                : Day("2701", "01") + "\n" + Day("2701", "02")));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var result = await sut.GetDstAsync(new DateOnly(2026, 12, 31), new DateOnly(2027, 1, 1), TestContext.Current.CancellationToken);

        // Assert
        handler.Requests.ShouldBe([
            new Uri("https://wdc.test/dst_realtime/202612/dst2612.for.request"),
            new Uri("https://wdc.test/dst_realtime/202701/dst2701.for.request")]);
        result.Count.ShouldBe(48);
        result[0].DateTime.ShouldBe(new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc));
        result[^1].DateTime.ShouldBe(new DateTime(2027, 1, 1, 23, 0, 0, DateTimeKind.Utc));
    }

    [Theory(DisplayName = "Dst returns no records for an absent file or an empty response")]
    [InlineData(HttpStatusCode.NotFound, "Not Found")]
    [InlineData(HttpStatusCode.OK, "")]
    public async Task GetDstAsync_WhenNoData_ReturnsEmpty(HttpStatusCode status, string content)
    {
        // Arrange
        using var handler = new TestHandler(_ => Response(status, content));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var result = await sut.GetDstAsync(new DateOnly(2026, 11, 1), TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Dst skips an unpublished month and still imports the next month")]
    public async Task GetDstAsync_WhenMonthAbsent_ContinuesRange()
    {
        // Arrange
        using var handler = new TestHandler(uri => uri.AbsolutePath.Contains("202610", StringComparison.Ordinal)
            ? Response(HttpStatusCode.NotFound, "") : Response(HttpStatusCode.OK, Day("2611", "01")));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var result = await sut.GetDstAsync(new DateOnly(2026, 10, 31), new DateOnly(2026, 11, 1), TestContext.Current.CancellationToken);

        // Assert
        handler.Requests.Count.ShouldBe(2);
        result.Count.ShouldBe(24);
    }

    [Fact(DisplayName = "Dst throws for HTTP errors other than an absent month")]
    public async Task GetDstAsync_WhenServerError_Throws()
    {
        // Arrange
        using var handler = new TestHandler(_ => Response(HttpStatusCode.InternalServerError, "error"));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var exception = await Should.ThrowAsync<HttpRequestException>(() => sut.GetDstAsync(new DateOnly(2026, 11, 1), TestContext.Current.CancellationToken));

        // Assert
        exception.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact(DisplayName = "Dst rejects a reversed range before sending requests")]
    public async Task GetDstAsync_WhenReversedRange_Throws()
    {
        // Arrange
        using var handler = new TestHandler(_ => Response(HttpStatusCode.OK, ""));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        await Should.ThrowAsync<ArgumentOutOfRangeException>(() => sut.GetDstAsync(new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 1), TestContext.Current.CancellationToken));

        // Assert
        handler.Requests.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Dst propagates cancellation without requesting another month")]
    public async Task GetDstAsync_WhenCanceled_StopsRange()
    {
        // Arrange
        using var source = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        using var handler = new TestHandler(_ =>
        {
            source.Cancel();
            return Response(HttpStatusCode.NotFound, "");
        });
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        await Should.ThrowAsync<OperationCanceledException>(() => sut.GetDstAsync(new DateOnly(2026, 10, 1), new DateOnly(2026, 11, 1), source.Token));

        // Assert
        handler.Requests.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "Dst range supports the last representable date without overflowing")]
    public async Task GetDstAsync_WhenMaxDate_DoesNotOverflow()
    {
        // Arrange
        using var handler = new TestHandler(_ => Response(HttpStatusCode.NotFound, ""));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var result = await sut.GetDstAsync(DateOnly.MaxValue, DateOnly.MaxValue, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeEmpty();
        handler.Requests.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "Dst rejects ranges spanning more than two calendar months before sending requests")]
    public async Task GetDstAsync_WhenRangeExceedsTwoMonths_ThrowsBeforeRequest()
    {
        // Arrange
        using var handler = new TestHandler(_ => Response(HttpStatusCode.NotFound, ""));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        await Should.ThrowAsync<ArgumentOutOfRangeException>(() => sut.GetDstAsync(
            new DateOnly(2026, 9, 30),
            new DateOnly(2026, 11, 1),
            TestContext.Current.CancellationToken));

        // Assert
        handler.Requests.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Dst requests a month once for a multi-day range within that month")]
    public async Task GetDstAsync_WhenRangeInOneMonth_RequestsFileOnce()
    {
        // Arrange
        using var handler = new TestHandler(_ => Response(HttpStatusCode.OK,
            Day("2611", "01") + "\n" + Day("2611", "02")));
        using var http = new HttpClient(handler);
        var sut = CreateSut(http);

        // Act
        var result = await sut.GetDstAsync(
            new DateOnly(2026, 11, 1),
            new DateOnly(2026, 11, 2),
            TestContext.Current.CancellationToken);

        // Assert
        handler.Requests.Count.ShouldBe(1);
        result.Count.ShouldBe(48);
    }

    private static IDstIndexClient CreateSut(HttpClient http)
        => new DstIndexClient(http, Options.Create(new WdcClientOptions { ServerUrl = new Uri("https://wdc.test/") }));

    private static string Day(string yearMonth, string day)
        => "DST" + yearMonth + "*" + day + "RRX020   0" + string.Concat(Enumerable.Repeat("  -1", 25));

    private static HttpResponseMessage Response(HttpStatusCode status, string content)
        => new(status) { Content = new StringContent(content) };

    private sealed class TestHandler(Func<Uri, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var uri = request.RequestUri ?? throw new InvalidOperationException("Request URI is missing.");
            Requests.Add(uri);
            return Task.FromResult(responseFactory(uri));
        }
    }
}
