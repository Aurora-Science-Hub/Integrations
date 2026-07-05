namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;

/// <summary>
/// 1-minute averaged real-time interplanetary magnetic field values (RTSW).
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="Bx">IMF GSM x-component</param>
/// <param name="By">IMF GSM y-component</param>
/// <param name="Bz">IMF GSM z-component</param>
/// <param name="Bt">IMF module</param>
/// <param name="Latitude">Spacecraft latitude</param>
/// <param name="Longitude">Spacecraft longitude</param>
/// <param name="Active">Record activity state reported by NOAA</param>
/// <param name="Source">Record source reported by NOAA</param>
public record MagnetometerRecord(
    DateTime DateTime,
    float? Bx,
    float? By,
    float? Bz,
    float? Bt,
    float? Latitude,
    float? Longitude,
    bool Active,
    string Source);
