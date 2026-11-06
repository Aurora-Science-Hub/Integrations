using AuroraScienceHub.Framework.Http;
using AuroraScienceHub.Integrations.FmiClient.FmiElectrojet.Extensions;
using AuroraScienceHub.Integrations.FmiClient.FmiElectrojet.Responses;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.FmiClient.FmiElectrojet;

/// <inheritdoc />
internal sealed class FmiElectrojetClient : IFmiElectrojetClient
{
    private const string RealtimeIndicesPath = "image/realtime/eurisgic/realtime_iu_il.txt";

    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    public FmiElectrojetClient(HttpClient httpClient, IOptions<FmiClientOptions> options)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<IReadOnlyList<FmiElectrojetRecord>> GetIndicesAsync(CancellationToken cancellationToken)
    {
        var url = new Uri(_baseUrl, RealtimeIndicesPath);
        using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        await response.EnsureSuccess().ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return FmiElectrojetTextParser.Parse(text);
    }
}
