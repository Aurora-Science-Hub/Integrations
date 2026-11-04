using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;

namespace AuroraScienceHub.Integrations.WdcClient.DstIndex;

/// <summary>
/// World Data Center for Geomagnetism, Kyoto real-time Dst client
/// </summary>
/// <remarks>
/// Quick-look values are preliminary and can change. WDC does not guarantee publication latency.
/// Only months available in the real-time archive are requested, without fallback to provisional or final data
/// </remarks>
public interface IDstIndexClient
{
    /// <summary>
    /// Gets hourly Dst for a UTC day, ordered by time
    /// </summary>
    /// <param name="date">UTC day</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Published records, including nulls for missing or unpublished hours.
    /// An absent month file (HTTP 404) or an empty body returns an empty list
    /// </returns>
    Task<IReadOnlyList<DstIndexRecord>> GetDstAsync(DateOnly date, CancellationToken cancellationToken);

    /// <summary>
    /// Gets hourly Dst for an inclusive range of UTC days, ordered by time
    /// </summary>
    /// <param name="from">First UTC day, inclusive</param>
    /// <param name="to">Last UTC day, inclusive</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Records in the range, with missing values mapped to null and absent months skipped</returns>
    /// <remarks>
    /// The range may intersect at most two calendar months, matching the current and previous real-time months.
    /// Each month file is requested once, sequentially, regardless of the number of requested days
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">From is later than to, or the range intersects more than two calendar months</exception>
    Task<IReadOnlyList<DstIndexRecord>> GetDstAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken);
}
