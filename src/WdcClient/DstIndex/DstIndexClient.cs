using System.Globalization;
using System.Net;
using AuroraScienceHub.Framework.Http;
using AuroraScienceHub.Integrations.WdcClient.DstIndex.Extensions;
using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.WdcClient.DstIndex;

/// <inheritdoc />
internal sealed class DstIndexClient : IDstIndexClient
{
    private const int MaxRangeMonths = 2;
    private const int MonthsPerYear = 12;
    private const string RealtimePath = "dst_realtime/";
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    public DstIndexClient(HttpClient httpClient, IOptions<WdcClientOptions> options)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public Task<IReadOnlyList<DstIndexRecord>> GetDstAsync(DateOnly date, CancellationToken cancellationToken)
        => GetDstAsync(date, date, cancellationToken);

    public async Task<IReadOnlyList<DstIndexRecord>> GetDstAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to);
        var months = (to.Year - from.Year) * MonthsPerYear + to.Month - from.Month + 1;
        ArgumentOutOfRangeException.ThrowIfGreaterThan(months, MaxRangeMonths, nameof(to));

        var records = new List<DstIndexRecord>();
        var month = new DateOnly(from.Year, from.Month, 1);
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var path = string.Create(CultureInfo.InvariantCulture, $"{RealtimePath}{month:yyyyMM}/dst{month:yyMM}.for.request");
            using var response = await _httpClient.GetAsync(new Uri(_baseUrl, path), cancellationToken).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                await response.EnsureSuccess().ConfigureAwait(false);
                var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                records.AddRange(DstIndexWdcParser.Parse(text).Where(record =>
                {
                    var day = DateOnly.FromDateTime(record.DateTime);
                    return day >= from && day <= to && day.Year == month.Year && day.Month == month.Month;
                }));
            }

            if (month.Year == to.Year && month.Month == to.Month)
            {
                break;
            }

            month = month.AddMonths(1);
        }

        return records;
    }
}
