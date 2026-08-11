using System.Net;
using System.Text.Json;
using AuroraScienceHub.Framework.Http;
using AuroraScienceHub.Framework.Json;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;
using AuroraScienceHub.Integrations.NoaaClient.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw;

/// <inheritdoc />
internal sealed class RtswClient : IRtswClient
{
    private const string WafActionHeaderName = "x-amzn-waf-action";
    private const string WafChallengeAction = "challenge";

    private static readonly JsonSerializerOptions s_jsonOptions = DefaultJsonSerializerOptions.Create();

    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RtswClient"/> class.
    /// </summary>
    public RtswClient(
        HttpClient httpClient,
        IOptions<NoaaClientOptions> options,
        ILogger<RtswClient> logger)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "json/rtsw/rtsw_mag_1m.json");
        return await GetRtswJsonAsync<List<MagnetometerRecord>>(url, cancellationToken).ConfigureAwait(false)
               ?? [];
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "json/rtsw/rtsw_wind_1m.json");
        return await GetRtswJsonAsync<List<SolarWindPlasmaRecord>>(url, cancellationToken).ConfigureAwait(false)
               ?? [];
    }

    private async Task<TResponse?> GetRtswJsonAsync<TResponse>(
        Uri requestUri,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        return await ReadRtswJsonOrThrowAsync<TResponse>(response, requestUri, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TResponse?> ReadRtswJsonOrThrowAsync<TResponse>(
        HttpResponseMessage response,
        Uri requestUri,
        CancellationToken cancellationToken)
    {
        var wafAction = GetWafAction(response);
        var contentLength = response.Content.Headers.ContentLength;

        _logger.LogInformation(
            "RTSW HTTP response Uri={RequestUri} StatusCode={StatusCode} WafAction={WafAction} ContentLength={ContentLength}",
            requestUri,
            (int)response.StatusCode,
            wafAction,
            contentLength);

        if (response.StatusCode == HttpStatusCode.Accepted
            && string.Equals(wafAction, WafChallengeAction, StringComparison.OrdinalIgnoreCase))
        {
            throw new HttpRequestException(
                $"NOAA RTSW request blocked by AWS WAF challenge (HTTP {(int)response.StatusCode}, {WafActionHeaderName}={wafAction}, Uri={requestUri}).",
                inner: null,
                statusCode: response.StatusCode);
        }

        await response.EnsureSuccess().ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NoContent || contentLength == 0)
        {
            throw new HttpRequestException(
                $"NOAA RTSW returned an empty response body (HTTP {(int)response.StatusCode}, ContentLength={contentLength}, Uri={requestUri}).",
                inner: null,
                statusCode: response.StatusCode);
        }

        var rawBytes = await response.Content
            .ReadAsByteArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var sanitized = NoaaJsonSanitizer.Sanitize(rawBytes);
        if (sanitized.ReplacementCount > 0)
        {
            _logger.LogWarning(
                "NOAA RTSW JSON sanitized {ReplacementCount} non-standard numeric literals to null. Uri={RequestUri}",
                sanitized.ReplacementCount,
                requestUri);
        }

        return JsonSerializer.Deserialize<TResponse>(sanitized.Bytes.Span, s_jsonOptions);
    }

    private static string? GetWafAction(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues(WafActionHeaderName, out var values))
        {
            return values.FirstOrDefault();
        }

        if (response.Content.Headers.TryGetValues(WafActionHeaderName, out values))
        {
            return values.FirstOrDefault();
        }

        return null;
    }
}
