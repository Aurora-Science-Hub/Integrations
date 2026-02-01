using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Dscovr.Extensions;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Dscovr.Responses;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Dscovr;

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
    => await GetMagnetometerDataAsync("/products/solar-wind/mag-2-hour.json", cancellationToken)
        .ConfigureAwait(false);

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData1DAsync(CancellationToken cancellationToken)
    => await GetMagnetometerDataAsync("/products/solar-wind/mag-1-day.json", cancellationToken)
        .ConfigureAwait(false);

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData3DAsync(CancellationToken cancellationToken)
    => await GetMagnetometerDataAsync("/products/solar-wind/mag-3-day.json", cancellationToken)
        .ConfigureAwait(false);

    public async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData7DAsync(CancellationToken cancellationToken)
    => await GetMagnetometerDataAsync("/products/solar-wind/mag-7-day.json", cancellationToken)
        .ConfigureAwait(false);

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData2HAsync(CancellationToken cancellationToken)
    => await GetSolarWindPlasmaDataAsync("/products/solar-wind/plasma-2-hour.json", cancellationToken)
        .ConfigureAwait(false);

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData1DAsync(CancellationToken cancellationToken)
    => await GetSolarWindPlasmaDataAsync("/products/solar-wind/plasma-1-day.json", cancellationToken)
        .ConfigureAwait(false);

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData3DAsync(CancellationToken cancellationToken)
    => await GetSolarWindPlasmaDataAsync("/products/solar-wind/plasma-3-day.json", cancellationToken)
        .ConfigureAwait(false);

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData7DAsync(CancellationToken cancellationToken)
    => await GetSolarWindPlasmaDataAsync("/products/solar-wind/plasma-7-day.json", cancellationToken)
        .ConfigureAwait(false);

    private async Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerDataAsync(string relativeUri, CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, relativeUri);
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken);

        return MagnetometerDataParser.Parse(text);
    }

    private async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaDataAsync(string relativeUri, CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, relativeUri);
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return SolarWindPlasmaDataParser.Parse(text);
    }
}
