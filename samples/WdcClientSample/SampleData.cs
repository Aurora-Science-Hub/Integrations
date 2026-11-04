using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;

namespace AuroraScienceHub.Integrations.Samples.WdcClientSample;

internal static class SampleData
{
    /// <summary>
    /// First UTC date supported by the real-time sample
    /// </summary>
    public static DateOnly GetEarliestDate(DateOnly today)
        => new DateOnly(today.Year, today.Month, 1).AddMonths(-1);

    /// <summary>
    /// Whether both inclusive range endpoints belong to the current real-time window
    /// </summary>
    public static bool IsSupportedRange(DateOnly from, DateOnly to, DateOnly today)
        => from >= GetEarliestDate(today)
           && from <= to
           && to <= today;

    /// <summary>
    /// Records through the current UTC time, preserving missing measurements in elapsed hours
    /// </summary>
    public static IReadOnlyList<DstIndexRecord> GetElapsedHours(IReadOnlyList<DstIndexRecord> records, DateTime utcNow)
        => records.Where(record => record.DateTime <= utcNow).ToList();
}
