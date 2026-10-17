using System.Net;
using AuroraScienceHub.Integrations.DtuSpaceClient;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.DtuSpaceClient.PcIndex;

/// <summary>
/// Unit tests for <see cref="PcIndexClient"/>.
/// </summary>
public sealed class PcIndexClientTests
{
    private static readonly Uri BaseUrl = new("https://dtu.test/");

    [Fact(DisplayName = "GetPcn requests the quick-look CSV file for the specified day")]
    public async Task GetPcnAsync_WhenCalled_RequestsQuickLookDayFile()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(CreateResponse(HttpStatusCode.OK, "date,pcn\n"));
        var sut = CreateSut(handler);

        // Act
        await sut.GetPcnAsync(new DateOnly(2026, 10, 7), TestContext.Current.CancellationToken);

        // Assert
        handler.RequestUris.ShouldBe([new Uri("https://dtu.test/WDC/indices/pcn/PCN_quicklook/pcn20261007q.csv")]);
    }

    [Fact(DisplayName = "GetPcn returns parsed records for successful response")]
    public async Task GetPcnAsync_WhenSuccess_ReturnsRecords()
    {
        // Arrange
        const string csv = "date,pcn\n2026-10-17 00:00:00.000,-0.29\n2026-10-17 00:01:00.000,NaN\n";
        var sut = CreateSut(new TestHttpMessageHandler(CreateResponse(HttpStatusCode.OK, csv)));

        // Act
        var records = await sut.GetPcnAsync(new DateOnly(2026, 10, 17), TestContext.Current.CancellationToken);

        // Assert
        records.Count.ShouldBe(2);
        records[0].Value.ShouldBe(-0.29f);
        records[1].Value.ShouldBeNull();
    }

    [Fact(DisplayName = "GetPcn returns an empty list when the day file is not published")]
    public async Task GetPcnAsync_WhenNotFound_ReturnsEmpty()
    {
        // Arrange
        var sut = CreateSut(new TestHttpMessageHandler(CreateResponse(HttpStatusCode.NotFound, "Not Found")));

        // Act
        var records = await sut.GetPcnAsync(new DateOnly(2026, 10, 20), TestContext.Current.CancellationToken);

        // Assert
        records.ShouldBeEmpty();
    }

    [Fact(DisplayName = "GetPcn returns an empty list for an empty body")]
    public async Task GetPcnAsync_WhenEmptyBody_ReturnsEmpty()
    {
        // Arrange
        var sut = CreateSut(new TestHttpMessageHandler(CreateResponse(HttpStatusCode.OK, string.Empty)));

        // Act
        var records = await sut.GetPcnAsync(new DateOnly(2026, 10, 17), TestContext.Current.CancellationToken);

        // Assert
        records.ShouldBeEmpty();
    }

    [Fact(DisplayName = "GetPcn throws on server error")]
    public async Task GetPcnAsync_WhenServerError_ThrowsHttpRequestException()
    {
        // Arrange
        var sut = CreateSut(new TestHttpMessageHandler(CreateResponse(HttpStatusCode.InternalServerError, "error")));

        // Act
        var exception = await Should.ThrowAsync<HttpRequestException>(
            async () => await sut.GetPcnAsync(new DateOnly(2026, 10, 17), TestContext.Current.CancellationToken));

        // Assert
        exception.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact(DisplayName = "GetPcn for a range requests every day file and returns records in time order")]
    public async Task GetPcnAsync_WhenRange_ReturnsRecordsOfAllDaysInOrder()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(uri => uri.AbsolutePath switch
        {
            var path when path.EndsWith("pcn20261016q.csv", StringComparison.Ordinal)
                => CreateResponse(HttpStatusCode.OK, "date,pcn\n2026-10-16 23:59:00.000,-0.26\n"),
            var path when path.EndsWith("pcn20261017q.csv", StringComparison.Ordinal)
                => CreateResponse(HttpStatusCode.OK, "date,pcn\n2026-10-17 00:00:00.000,-0.29\n"),
            _ => CreateResponse(HttpStatusCode.NotFound, "Not Found"),
        });
        var sut = CreateSut(handler);

        // Act
        var records = await sut.GetPcnAsync(
            new DateOnly(2026, 10, 16),
            new DateOnly(2026, 10, 18),
            TestContext.Current.CancellationToken);

        // Assert
        handler.RequestUris.Select(uri => uri.Segments[^1]).ShouldBe([
            "pcn20261016q.csv",
            "pcn20261017q.csv",
            "pcn20261018q.csv",
        ]);
        records.Select(record => record.DateTime).ShouldBe([
            new DateTime(2026, 10, 16, 23, 59, 0, DateTimeKind.Utc),
            new DateTime(2026, 10, 17, 0, 0, 0, DateTimeKind.Utc),
        ]);
    }

    [Fact(DisplayName = "GetPcn for a single-day range requests only that day file")]
    public async Task GetPcnAsync_WhenRangeOfOneDay_RequestsSingleFile()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(CreateResponse(HttpStatusCode.OK, "date,pcn\n"));
        var sut = CreateSut(handler);
        var date = new DateOnly(2026, 10, 17);

        // Act
        await sut.GetPcnAsync(date, date, TestContext.Current.CancellationToken);

        // Assert
        handler.RequestUris.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "GetPcn for a range throws when the start day is later than the end day")]
    public async Task GetPcnAsync_WhenFromIsLaterThanTo_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(CreateResponse(HttpStatusCode.OK, "date,pcn\n"));
        var sut = CreateSut(handler);

        // Act
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            async () => await sut.GetPcnAsync(
                new DateOnly(2026, 10, 17),
                new DateOnly(2026, 10, 16),
                TestContext.Current.CancellationToken));

        // Assert
        handler.RequestUris.ShouldBeEmpty();
    }

    [Fact(DisplayName = "GetPcn for a range of the archive depth requests a file per day")]
    public async Task GetPcnAsync_WhenRangeOfMaxLength_RequestsFilePerDay()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(_ => CreateResponse(HttpStatusCode.NotFound, string.Empty));
        var sut = CreateSut(handler);
        var to = new DateOnly(2026, 10, 17);

        // Act
        await sut.GetPcnAsync(to.AddDays(-30), to, TestContext.Current.CancellationToken);

        // Assert
        handler.RequestUris.Count.ShouldBe(31);
    }

    [Fact(DisplayName = "GetPcn for a range throws when the range is longer than the archive depth")]
    public async Task GetPcnAsync_WhenRangeIsLongerThanMaxLength_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(_ => CreateResponse(HttpStatusCode.NotFound, string.Empty));
        var sut = CreateSut(handler);
        var to = new DateOnly(2026, 10, 17);

        // Act
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            async () => await sut.GetPcnAsync(to.AddDays(-31), to, TestContext.Current.CancellationToken));

        // Assert
        handler.RequestUris.ShouldBeEmpty();
    }

    [Fact(DisplayName = "GetPcn for a range ending on the last representable day does not overflow")]
    public async Task GetPcnAsync_WhenRangeEndsOnMaxValue_RequestsSingleFile()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(_ => CreateResponse(HttpStatusCode.NotFound, string.Empty));
        var sut = CreateSut(handler);

        // Act
        var result = await sut.GetPcnAsync(DateOnly.MaxValue, DateOnly.MaxValue, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeEmpty();
        handler.RequestUris.Count.ShouldBe(1);
    }

    private static IPcIndexClient CreateSut(TestHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        var options = Options.Create(new DtuSpaceClientOptions { ServerUrl = BaseUrl });
        return new PcIndexClient(httpClient, options);
    }

    private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string content)
        => new(statusCode) { Content = new StringContent(content) };

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<Uri, HttpResponseMessage> _responseFactory;

        public TestHttpMessageHandler(HttpResponseMessage response)
            : this(_ => response)
        {
        }

        public TestHttpMessageHandler(Func<Uri, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public List<Uri> RequestUris { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var requestUri = request.RequestUri!;
            RequestUris.Add(requestUri);
            return Task.FromResult(_responseFactory(requestUri));
        }
    }
}
