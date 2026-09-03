using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AuroraScienceHub.Framework.Http;
using AuroraScienceHub.Integrations.NoaaClient.Http;
using AuroraScienceHub.Integrations.NoaaClient.Json;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;
using AuroraScienceHub.Integrations.NoaaClient.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw;

/// <inheritdoc />
internal sealed class RtswClient : IRtswClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;
    private readonly NoaaClientOptions _options;
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
        _options = options.Value;
        _baseUrl = _options.RequiredServerUrl;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "json/rtsw/rtsw_mag_1m.json");
        return await GetRtswJsonAsync(
                url,
                NoaaJsonSerializerContext.Default.ListMagnetometerRecord,
                cancellationToken)
            .ConfigureAwait(false) ?? [];
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "json/rtsw/rtsw_wind_1m.json");
        return await GetRtswJsonAsync(
                url,
                NoaaJsonSerializerContext.Default.ListSolarWindPlasmaRecord,
                cancellationToken)
            .ConfigureAwait(false) ?? [];
    }

    private async Task<TResponse?> GetRtswJsonAsync<TResponse>(
        Uri requestUri,
        JsonTypeInfo<TResponse> jsonTypeInfo,
        CancellationToken cancellationToken)
    {
        var attempts = Math.Max(1, _options.RtswRetryCount);
        JsonException? lastJsonException = null;

        for (var attempt = 1; attempt <= attempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
                return await ReadRtswJsonOrThrowAsync(response, requestUri, jsonTypeInfo, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (JsonException ex) when (attempt < attempts)
            {
                // NOAA occasionally serves a truncated/partial JSON body (connection cut
                // near EOF). Treat it as retryable: re-download the whole feed.
                lastJsonException = ex;
                _logger.LogWarning(
                    "RTSW JSON parse failed on attempt {Attempt}/{Attempts} for {RequestUri}: {Message}. Retrying.",
                    attempt,
                    attempts,
                    requestUri,
                    ex.Message);
                await Task.Delay(_options.RtswRetryDelay, cancellationToken).ConfigureAwait(false);
            }
        }

        throw lastJsonException
            ?? new JsonException($"RTSW feed {requestUri} could not be parsed after {attempts} attempts.");
    }

    private async Task<TResponse?> ReadRtswJsonOrThrowAsync<TResponse>(
        HttpResponseMessage response,
        Uri requestUri,
        JsonTypeInfo<TResponse> jsonTypeInfo,
        CancellationToken cancellationToken)
    {
        var wafAction = NoaaHttpResponseExtensions.GetWafAction(response);
        var contentLength = response.Content.Headers.ContentLength;

        _logger.LogInformation(
            "RTSW HTTP response Uri={RequestUri} StatusCode={StatusCode} WafAction={WafAction} ContentLength={ContentLength}",
            requestUri,
            (int)response.StatusCode,
            wafAction,
            contentLength);

        await response.EnsureNoaaSuccessAsync(requestUri, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            response.ThrowIfEmptyBody(ReadOnlySpan<byte>.Empty, requestUri);
        }

        var rawBytes = await response.Content
            .ReadAsByteArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        response.ThrowIfEmptyBody(rawBytes, requestUri);

        var sanitized = NoaaJsonSanitizer.Sanitize(rawBytes);
        if (sanitized.ReplacementCount > 0)
        {
            _logger.LogWarning(
                "NOAA RTSW JSON sanitized {ReplacementCount} non-standard numeric literals to null. Uri={RequestUri}",
                sanitized.ReplacementCount,
                requestUri);
        }

        return JsonSerializer.Deserialize(sanitized.Bytes.Span, jsonTypeInfo);
    }
}
