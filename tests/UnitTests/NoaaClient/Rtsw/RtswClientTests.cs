using System.Net;
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

    [Fact(DisplayName = "GetMagnetometerData accepts fractional sample_size (60.0)")]
    public async Task GetMagnetometerDataAsync_WhenFractionalSampleSize_DeserializesValue()
    {
        // Arrange
        var json = """
            [
              {
                "time_tag": "2026-10-29T15:28:00Z",
                "active": true,
                "source": "SOLAR1",
                "sample_size": 60.0,
                "bt": 3.18
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
        records[0].SampleSize.ShouldBe(60);
    }

    [Fact(DisplayName = "GetSolarWindPlasmaData accepts fractional proton_sample_size (1.0)")]
    public async Task GetSolarWindPlasmaDataAsync_WhenFractionalSampleSize_DeserializesValue()
    {
        // Arrange
        var json = """
            [
              {
                "time_tag": "2026-10-29T15:27:00Z",
                "active": true,
                "source": "SOLAR1",
                "proton_sample_size": 1.0,
                "proton_speed": 344.8
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
        records[0].ProtonSampleSize.ShouldBe(1);
    }

    [Fact(DisplayName = "GetMagnetometerData reads a non-integral sample_size as null")]
    public async Task GetMagnetometerDataAsync_WhenNonIntegralSampleSize_DeserializesNull()
    {
        // Arrange
        var json = """
            [
              {
                "time_tag": "2026-10-29T15:28:00Z",
                "active": true,
                "source": "SOLAR1",
                "sample_size": 60.5,
                "bt": 3.18
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
        records[0].SampleSize.ShouldBeNull();
    }

    [Fact(DisplayName = "GetMagnetometerData reads an out-of-range sample_size as null")]
    public async Task GetMagnetometerDataAsync_WhenSampleSizeOutOfRange_DeserializesNull()
    {
        // Arrange
        var json = """
            [
              {
                "time_tag": "2026-10-29T15:28:00Z",
                "active": true,
                "source": "SOLAR1",
                "sample_size": 3e9,
                "bt": 3.18
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
        records[0].SampleSize.ShouldBeNull();
    }

    [Fact(DisplayName = "GetSolarWindPlasmaData accepts a numeric-string proton_sample_size")]
    public async Task GetSolarWindPlasmaDataAsync_WhenSampleSizeIsNumericString_DeserializesValue()
    {
        // Arrange
        var json = """
            [
              {
                "time_tag": "2026-10-29T15:27:00Z",
                "active": true,
                "source": "SOLAR1",
                "proton_sample_size": "60",
                "proton_speed": 344.8
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
        records[0].ProtonSampleSize.ShouldBe(60);
    }

    [Fact(DisplayName = "GetSolarWindPlasmaData accepts fractional alpha_sample_size (2.0)")]
    public async Task GetSolarWindPlasmaDataAsync_WhenFractionalAlphaSampleSize_DeserializesValue()
    {
        // Arrange
        var json = """
            [
              {
                "time_tag": "2026-10-29T15:27:00Z",
                "active": true,
                "source": "SOLAR1",
                "alpha_sample_size": 2.0,
                "alpha_speed": 320.1
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
        records[0].AlphaSampleSize.ShouldBe(2);
    }

    private static IRtswClient CreateSut(HttpResponseMessage response)
    {
        var handler = new TestHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);
        var options = Options.Create(new NoaaClientOptions { ServerUrl = BaseUrl });
        return new RtswClient(httpClient, options, NullLogger<RtswClient>.Instance);
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public TestHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_response);
        }
    }
}
