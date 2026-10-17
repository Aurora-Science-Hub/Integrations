using System.Globalization;
using System.Net;
using AuroraScienceHub.Framework.Http;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Extensions;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Responses;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex;

/// <inheritdoc />
internal sealed class PcIndexClient : IPcIndexClient
{
    private const string PcnQuickLookPath = "WDC/indices/pcn/PCN_quicklook/";

    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    public PcIndexClient(
        HttpClient httpClient,
        IOptions<DtuSpaceClientOptions> options)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<IReadOnlyList<PcIndexRecord>> GetPcnAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var fileName = string.Create(CultureInfo.InvariantCulture, $"pcn{date:yyyyMMdd}q.csv");
        var url = new Uri(_baseUrl, PcnQuickLookPath + fileName);

        using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

        // The day file is absent before its first minutes are published and after it leaves the quick-look archive
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }

        await response.EnsureSuccess().ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return string.IsNullOrWhiteSpace(text) ? [] : PcIndexCsvParser.Parse(text);
    }

    public async Task<IReadOnlyList<PcIndexRecord>> GetPcnAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to);

        var records = new List<PcIndexRecord>();
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            records.AddRange(await GetPcnAsync(date, cancellationToken).ConfigureAwait(false));
        }

        return records;
    }
}
