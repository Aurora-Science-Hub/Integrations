namespace AuroraScienceHub.Integrations.Noaa.Dscovr.Responses;

/// <summary>
/// 1-minute averaged Real-time Interplanetary Magnetic Field Values (DSCOVR)
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="Bx">IMF GSM x-component</param>
/// <param name="By">IMF GSM y-component</param>
/// <param name="Bz">IMF GSM z-component</param>
/// <param name="Bt">IMF module</param>
/// <param name="Latitude">Spacecraft latitude</param>
/// <param name="Longitude">Spacecraft longitude</param>
public record MagnetometerRecord(
    DateTime DateTime,
    float? Bx,
    float? By,
    float? Bz,
    float? Bt,
    float? Latitude,
    float? Longitude);
