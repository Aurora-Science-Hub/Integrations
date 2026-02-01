namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.KpIndex.Responses;

/// <summary>
/// 3-day KP-index data
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="KpIndex">KP-index</param>
public sealed record KpIndex3DayResponse(DateTime DateTime, float KpIndex);
