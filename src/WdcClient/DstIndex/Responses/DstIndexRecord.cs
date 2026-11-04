namespace AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;

/// <summary>
/// Real-time hourly Dst index from World Data Center for Geomagnetism, Kyoto
/// </summary>
/// <param name="DateTime">Start of the hourly interval in UTC, from 00:00 through 23:00</param>
/// <param name="Value">Dst in nT, or null when WDC publishes the missing value 9999</param>
public sealed record DstIndexRecord(DateTime DateTime, int? Value);
