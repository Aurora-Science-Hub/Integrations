using System.Net;
using AuroraScienceHub.Framework.Http;

namespace AuroraScienceHub.Integrations.NoaaClient.Http;

/// <summary>
/// NOAA-specific HTTP response validation helpers.
/// </summary>
internal static class NoaaHttpResponseExtensions
{
    private const string WafActionHeaderName = "x-amzn-waf-action";
    private const string WafChallengeAction = "challenge";

    /// <summary>
    /// Ensures the NOAA response is not a WAF challenge and has a successful status code.
    /// </summary>
    public static async Task EnsureNoaaSuccessAsync(
        this HttpResponseMessage response,
        Uri? requestUri = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfWafChallenge(response, requestUri);
        await response.EnsureSuccess().ConfigureAwait(false);
    }

    /// <summary>
    /// Throws when the NOAA response body is empty after a successful status code.
    /// </summary>
    public static void ThrowIfEmptyBody(
        this HttpResponseMessage response,
        ReadOnlySpan<byte> body,
        Uri? requestUri = null)
    {
        if (body.Length > 0)
        {
            return;
        }

        var contentLength = response.Content.Headers.ContentLength;
        throw new HttpRequestException(
            $"NOAA returned an empty response body (HTTP {(int)response.StatusCode}, ContentLength={contentLength}, Uri={requestUri}).",
            inner: null,
            statusCode: response.StatusCode);
    }

    internal static string? GetWafAction(HttpResponseMessage response)
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

    private static void ThrowIfWafChallenge(HttpResponseMessage response, Uri? requestUri)
    {
        var wafAction = GetWafAction(response);
        if (response.StatusCode != HttpStatusCode.Accepted
            || !string.Equals(wafAction, WafChallengeAction, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new HttpRequestException(
            $"NOAA request blocked by AWS WAF challenge (HTTP {(int)response.StatusCode}, {WafActionHeaderName}={wafAction}, Uri={requestUri}).",
            inner: null,
            statusCode: response.StatusCode);
    }
}
