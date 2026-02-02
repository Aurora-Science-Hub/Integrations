using AuroraScienceHub.Integrations.Noaa.KpIndex.Extensions;
using AuroraScienceHub.Integrations.Noaa.KpIndex.Responses;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.Noaa.KpIndex;

internal sealed class KpIndexClient : IKpIndexClient
{
    private readonly HttpClient _client;
    private readonly Uri _baseUrl;

    public KpIndexClient(
        HttpClient client,
        IOptions<NoaaClientOptions> options)
    {
        _client = client;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<IReadOnlyList<KpIndex27DayResponse>> GetKpIndex27DayForecastAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "text/27-day-outlook.txt");
        var text = await GetStringOrDefaultAsync(url, cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(text) ? [] : KpIndex27DayDataParser.Parse(text);
    }

    public async Task<IReadOnlyList<KpIndex3DayResponse>> GetKpIndex3DayForecastAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/noaa-planetary-k-index-forecast.json");
        var text = await GetStringOrDefaultAsync(url, cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(text) ? [] : KpIndex3DayDataParser.Parse(text);
    }

    public async Task<IReadOnlyList<KpIndexNowcastResponse>> GetKpIndexNowcastAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/noaa-planetary-k-index.json");
        var text = await GetStringOrDefaultAsync(url, cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(text) ? [] : KpIndexNowcastDataParser.Parse(text);
    }

    private async Task<string?> GetStringOrDefaultAsync(Uri url, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
}
