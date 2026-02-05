using AuroraScienceHub.Integrations.NoaaClient.Dscovr.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Dscovr.Responses;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.NoaaClient.Dscovr;

internal sealed class DscovrClient : IDscovrClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    public DscovrClient(
        HttpClient httpClient,
        IOptions<NoaaClientOptions> options)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData2HAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/solar-wind/mag-2-hour.json");
        return await GetMagnetometerDataAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData1DAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/solar-wind/mag-1-day.json");
        return await GetMagnetometerDataAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData3DAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/solar-wind/mag-3-day.json");
        return await GetMagnetometerDataAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData7DAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/solar-wind/mag-7-day.json");
        return await GetMagnetometerDataAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData2HAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/solar-wind/plasma-2-hour.json");
        return await GetSolarWindPlasmaDataAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData1DAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/solar-wind/plasma-1-day.json");
        return await GetSolarWindPlasmaDataAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData3DAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/solar-wind/plasma-3-day.json");
        return await GetSolarWindPlasmaDataAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData7DAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "products/solar-wind/plasma-7-day.json");
        return await GetSolarWindPlasmaDataAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerDataAsync(
        Uri url,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken);

        return MagnetometerDataParser.Parse(text);
    }

    private async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaDataAsync(
        Uri url,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return SolarWindPlasmaDataParser.Parse(text);
    }
}
