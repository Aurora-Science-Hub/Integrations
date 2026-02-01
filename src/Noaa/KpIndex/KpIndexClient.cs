using AuroraScienceHub.Integrations.Noaa.KpIndex.Extensions;
using AuroraScienceHub.Integrations.Noaa.KpIndex.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.Noaa.KpIndex;

internal sealed class KpIndexClient : IKpIndexClient
{
    private readonly ILogger _logger;
    private readonly HttpClient _client;
    private readonly Uri _baseUrl;

    public KpIndexClient(
        ILogger<KpIndexClient> logger,
        HttpClient client,
        IOptions<NoaaClientOptions> options)
    {
        _logger = logger;
        _client = client;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<IReadOnlyList<KpIndex27DayResponse>> GetKpIndex27DayForecastAsync(CancellationToken cancellationToken)
    {
        var text = await GetStringOrDefaultAsync("text/27-day-outlook.txt", cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(text) ? [] : KpIndex27DayDataParser.Parse(text);
    }

    public async Task<IReadOnlyList<KpIndex3DayResponse>> GetKpIndex3DayForecastAsync(CancellationToken cancellationToken)
    {
        var text = await GetStringOrDefaultAsync("products/noaa-planetary-k-index-forecast.json", cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(text) ? [] : KpIndex3DayDataParser.Parse(text);
    }

    public async Task<IReadOnlyList<KpIndexNowcastResponse>> GetKpIndexNowcastAsync(CancellationToken cancellationToken)
    {
        var text = await GetStringOrDefaultAsync("products/noaa-planetary-k-index.json", cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(text) ? [] : KpIndexNowcastDataParser.Parse(text);
    }

    private async Task<string?> GetStringOrDefaultAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, relativeUrl);
        var response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
}
