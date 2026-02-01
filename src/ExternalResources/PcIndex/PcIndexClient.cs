using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Infrastructure;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Infrastructure.Configuration;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex.Extensions;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex.Responses;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex;

internal sealed class PcIndexClient : AariGeophysClientBase, IPcIndexClient
{
    protected override string BasePath => ApiResource.PcIndex.BasePath;

    public PcIndexClient(HttpClient client, IOptions<AariGeophysClientOptions> options)
        : base(client, options) { }

    public async Task<IReadOnlyList<PcIndexResponse>> GetPcNIndexAsync(CancellationToken cancellationToken)
        => await GetPcIndexAsync(ApiResource.PcIndex.PcN, cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<PcIndexResponse>> GetPcSIndexAsync(CancellationToken cancellationToken)
        => await GetPcIndexAsync(ApiResource.PcIndex.PcS, cancellationToken).ConfigureAwait(false);


    private async Task<IReadOnlyList<PcIndexResponse>> GetPcIndexAsync(string pcUrl, CancellationToken cancellationToken)
    {
        var requestUrl = GetPathToData(pcUrl);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        using var response = await SendRequestAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return string.IsNullOrWhiteSpace(content) ? [] : PcIndexDataParser.Parse(content);
    }
}
