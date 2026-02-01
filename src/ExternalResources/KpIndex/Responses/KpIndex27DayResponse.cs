namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.KpIndex.Responses;

/// <summary>
/// 27-day KP-index data
/// </summary>
/// <param name="Date">Date of record</param>
/// <param name="KpIndex">KP-index</param>
public sealed record KpIndex27DayResponse(
    DateOnly Date,
    int KpIndex);
