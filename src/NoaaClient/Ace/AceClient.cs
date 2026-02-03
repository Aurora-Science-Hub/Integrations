using AuroraScienceHub.Integrations.NoaaClient.Ace.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Ace.Responses;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.NoaaClient.Ace;

internal sealed class AceClient : IAceClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    public AceClient(
        HttpClient httpClient,
        IOptions<NoaaClientOptions> options)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "text/ace-magnetometer.txt");
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return MagnetometerDataParser.Parse(text);
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSwepamDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "text/ace-swepam.txt");
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return SolarWindPlasmaDataParser.Parse(text);
    }
}
