namespace AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Responses;

/// <summary>
/// 1-minute Polar Cap (PC) index value
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="Value">PC index value in mV/m; <see langword="null"/> when the value is missing</param>
public sealed record PcIndexRecord(DateTime DateTime, float? Value);
