using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw;

/// <inheritdoc />
internal sealed class RtswClient : IRtswClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="RtswClient"/> class.
    /// </summary>
    public RtswClient(
        HttpClient httpClient,
        IOptions<NoaaClientOptions> options)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "json/rtsw/rtsw_mag_1m.json");
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return MagnetometerDataParser.Parse(text);
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "json/rtsw/rtsw_wind_1m.json");
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return SolarWindPlasmaDataParser.Parse(text);
    }
}
