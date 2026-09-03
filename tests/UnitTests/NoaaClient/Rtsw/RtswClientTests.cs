using System.Linq;
using System.Net;
using System.Text.Json;
using AuroraScienceHub.Integrations.NoaaClient;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.Rtsw;

/// <summary>
/// Unit tests for <see cref="RtswClient"/> HTTP response hardening.
/// </summary>
public sealed class RtswClientTests
{
    private static readonly Uri BaseUrl = new("https://noaa.test/");

    [Fact(DisplayName = "GetMagnetometerData throws on HTTP 202 WAF challenge with empty body")]
    public async Task GetMagnetometerDataAsync_WhenWafChallenge202_ThrowsHttpRequestException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent(string.Empty)
        };
        response.Headers.TryAddWithoutValidation("x-amzn-waf-action", "challenge");
        response.Content.Headers.ContentLength = 0;

        var sut = CreateSut(response);

        // Act
        var exception = await Should.ThrowAsync<HttpRequestException>(
            async () => await sut.GetMagnetometerDataAsync(TestContext.Current.CancellationToken));

        // Assert
        exception.Message.ShouldContain("WAF");
        exception.StatusCode.ShouldBe(HttpStatusCode.Accepted);
    }

    [Fact(DisplayName = "GetSolarWindPlasmaData throws on successful response with empty body")]
    public async Task GetSolarWindPlasmaDataAsync_WhenEmptyBody_ThrowsHttpRequestException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Array.Empty<byte>())
        };
        response.Content.Headers.ContentLength = 0;
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var sut = CreateSut(response);

        // Act
        var exception = await Should.ThrowAsync<HttpRequestException>(
            async () => await sut.GetSolarWindPlasmaDataAsync(TestContext.Current.CancellationToken));

        // Assert
        exception.Message.ShouldContain("empty");
        exception.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "GetSolarWindPlasmaData deserializes bare NaN as null")]
    public async Task GetSolarWindPlasmaDataAsync_WhenBareNaN_DeserializesNullSpeed()
    {
        // Arrange
        var json = """
            [
              {
                "time_tag": "2026-08-10T09:45:00Z",
                "active": true,
                "source": "SOLAR1",
                "proton_speed": NaN,
                "proton_density": 5.0,
                "proton_temperature": 100000.0
              }
            ]
            """;
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        var sut = CreateSut(response);

        // Act
        var records = await sut.GetSolarWindPlasmaDataAsync(TestContext.Current.CancellationToken);

        // Assert
        records.Count.ShouldBe(1);
        records[0].ProtonSpeed.ShouldBeNull();
        records[0].ProtonDensity.ShouldBe(5f);
    }

    [Fact(DisplayName = "GetSolarWindPlasmaData throws on chunked empty body without Content-Length")]
    public async Task GetSolarWindPlasmaDataAsync_WhenChunkedEmptyBody_ThrowsHttpRequestException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Array.Empty<byte>())
        };
        response.Content.Headers.ContentLength = null;

        var sut = CreateSut(response);

        // Act
        var exception = await Should.ThrowAsync<HttpRequestException>(
            async () => await sut.GetSolarWindPlasmaDataAsync(TestContext.Current.CancellationToken));

        // Assert
        exception.Message.ShouldContain("empty");
    }

    [Fact(DisplayName = "GetMagnetometerData returns records for valid JSON body")]
    public async Task GetMagnetometerDataAsync_WhenValidJson_ReturnsRecords()
    {
        // Arrange
        var json = """
            [
              {
                "time_tag": "2026-08-10T09:45:00Z",
                "active": true,
                "source": "SOLAR1",
                "bt": 5.0,
                "bx_gsm": 1.0,
                "by_gsm": 2.0,
                "bz_gsm": 3.0
              }
            ]
            """;
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        var sut = CreateSut(response);

        // Act
        var records = await sut.GetMagnetometerDataAsync(TestContext.Current.CancellationToken);

        // Assert
        records.Count.ShouldBe(1);
        records[0].Source.ShouldBe("SOLAR1");
        records[0].Bt.ShouldBe(5f);
    }

    [Fact(DisplayName = "GetMagnetometerData retries when JSON body is truncated, then succeeds")]
    public async Task GetMagnetometerDataAsync_WhenTruncatedJsonThenValid_RetriesAndReturns()
    {
        // Arrange
        var truncated = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""[{"time_tag":"2026-08-10T09:45:00Z","active":true,"source":"SOLAR1","bt":5.0,"bx_gsm":1.0,"by_gsm":2.0,"bz_gsm":3.0}""", System.Text.Encoding.UTF8, "application/json")
        };
        var valid = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """[{"time_tag":"2026-08-10T09:45:00Z","active":true,"source":"SOLAR1","bt":5.0,"bx_gsm":1.0,"by_gsm":2.0,"bz_gsm":3.0}]""",
                System.Text.Encoding.UTF8,
                "application/json")
        };

        var sut = CreateSut([truncated, valid]);

        // Act
        var records = await sut.GetMagnetometerDataAsync(TestContext.Current.CancellationToken);

        // Assert
        records.Count.ShouldBe(1);
        records[0].Source.ShouldBe("SOLAR1");
    }

    [Fact(DisplayName = "GetSolarWindPlasmaData throws when every JSON attempt is truncated")]
    public async Task GetSolarWindPlasmaDataAsync_WhenAllAttemptsTruncated_ThrowsJsonException()
    {
        // Arrange
        var truncated = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""[{"time_tag":"2026-08-10T09:45:00Z","active":true,"source":"SOLAR1","proton_speed":400.0}""", System.Text.Encoding.UTF8, "application/json")
        };
        var options = Options.Create(new NoaaClientOptions
        {
            ServerUrl = BaseUrl,
            RtswRetryCount = 2,
            RtswRetryDelay = TimeSpan.Zero,
        });
        var handler = new TestHttpMessageHandler([truncated, truncated]);
        var httpClient = new HttpClient(handler);
        var sut = new RtswClient(httpClient, options, NullLogger<RtswClient>.Instance);

        // Act
        var exception = await Should.ThrowAsync<JsonException>(
            async () => await sut.GetSolarWindPlasmaDataAsync(TestContext.Current.CancellationToken));

        // Assert
        exception.Message.ShouldContain("JSON");
        handler.CallCount.ShouldBe(2);
    }

    private static IRtswClient CreateSut(HttpResponseMessage response)
    {
        return CreateSut([response]);
    }

    private static IRtswClient CreateSut(IReadOnlyList<HttpResponseMessage> responses)
    {
        var handler = new TestHttpMessageHandler(responses);
        var httpClient = new HttpClient(handler);
        var options = Options.Create(new NoaaClientOptions { ServerUrl = BaseUrl });
        return new RtswClient(httpClient, options, NullLogger<RtswClient>.Instance);
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly IReadOnlyList<Func<HttpResponseMessage>> _responseFactories;
        private int _callCount;

        public int CallCount => _callCount;

        public TestHttpMessageHandler(HttpResponseMessage response)
            : this([() => CloneForDelivery(response)])
        {
        }

        public TestHttpMessageHandler(IReadOnlyList<HttpResponseMessage> responses)
            : this(responses.Select(r => (Func<HttpResponseMessage>)(() => CloneForDelivery(r))).ToArray())
        {
        }

        private TestHttpMessageHandler(IReadOnlyList<Func<HttpResponseMessage>> responseFactories)
        {
            _responseFactories = responseFactories;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var index = Math.Min(_callCount, _responseFactories.Count - 1);
            _callCount++;
            return Task.FromResult(_responseFactories[index]());
        }

        // The client disposes each HttpResponseMessage after reading it, so every
        // delivery must get a fresh instance with the same content.
        private static HttpResponseMessage CloneForDelivery(HttpResponseMessage source)
        {
            var body = source.Content?.ReadAsByteArrayAsync().GetAwaiter().GetResult() ?? [];
            var clone = new HttpResponseMessage(source.StatusCode)
            {
                Content = new ByteArrayContent(body)
            };
            if (source.Content?.Headers.ContentType is not null)
            {
                clone.Content.Headers.ContentType = source.Content.Headers.ContentType;
            }

            foreach (var header in source.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }
    }
}
