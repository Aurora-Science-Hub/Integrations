using AuroraScienceHub.Integrations.NoaaClient.Ace.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Ace.Responses;
using AuroraScienceHub.Integrations.NoaaClient.Http;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.NoaaClient.Ace;

/// <inheritdoc />
#pragma warning disable CS0618 // Implements obsolete IAceClient until removal in issue #3.
internal sealed class AceClient : IAceClient
#pragma warning restore CS0618
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
        using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        await response.EnsureNoaaSuccessAsync(url, cancellationToken).ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return MagnetometerDataParser.Parse(text);
    }

    public async Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSwepamDataAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, "text/ace-swepam.txt");
        using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        await response.EnsureNoaaSuccessAsync(url, cancellationToken).ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return SolarWindPlasmaDataParser.Parse(text);
    }
}
