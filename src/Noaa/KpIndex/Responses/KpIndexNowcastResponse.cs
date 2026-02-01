namespace AuroraScienceHub.Integrations.Noaa.KpIndex.Responses;

/// <summary>
/// Current KP-index data
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="KpIndex">KP-index</param>
/// <param name="StationsCount">KP-index calculated by stations count</param>
public sealed record KpIndexNowcastResponse(
    DateTime DateTime,
    float KpIndex,
    int StationsCount);
