using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Responses;

namespace AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex;

/// <summary>
/// DTU Space Polar Cap (PC) index client
/// </summary>
public interface IPcIndexClient
{
    /// <summary>
    /// Get 1-minute Polar Cap North (PCN) index for the specified UTC day
    /// </summary>
    /// <param name="date">UTC day of the data file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Returns near-real-time quick-look values: preliminary, published with a lag of about 10 minutes
    /// and retained by DTU Space for roughly the last 31 days.
    /// The current day file is appended during the day.
    /// Returns an empty list when the file for the day is not published (yet or anymore).
    /// </remarks>
    Task<IReadOnlyList<PcIndexRecord>> GetPcnAsync(DateOnly date, CancellationToken cancellationToken);

    /// <summary>
    /// Get 1-minute Polar Cap North (PCN) index for the specified range of UTC days
    /// </summary>
    /// <param name="from">First UTC day of the range (inclusive)</param>
    /// <param name="to">Last UTC day of the range (inclusive)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Returns near-real-time quick-look values ordered by time, see <see cref="GetPcnAsync(DateOnly, CancellationToken)"/>.
    /// Days whose files are not published are skipped.
    /// Day files are requested sequentially, one request per day.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="from"/> is later than <paramref name="to"/>.</exception>
    Task<IReadOnlyList<PcIndexRecord>> GetPcnAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
}
