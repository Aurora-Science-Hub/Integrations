namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;

/// <summary>
/// 1-minute averaged real-time bulk parameters of the solar wind plasma (RTSW).
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="ProtonDensity">Solar wind proton density</param>
/// <param name="BulkSpeed">Solar wind plasma speed</param>
/// <param name="IonTemperature">Solar wind ion temperature</param>
/// <param name="Active">Record activity state reported by NOAA</param>
/// <param name="Source">Record source reported by NOAA</param>
public record SolarWindPlasmaRecord(
    DateTime DateTime,
    float? ProtonDensity,
    float? BulkSpeed,
    float? IonTemperature,
    bool Active,
    string Source);
