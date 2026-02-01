namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex.Responses;

/// <summary>
/// PC-index data
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="Pc">Northern or Southern PC-index</param>
public sealed record PcIndexResponse(DateTime DateTime, float? Pc);
