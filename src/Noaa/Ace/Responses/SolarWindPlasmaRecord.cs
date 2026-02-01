namespace AuroraScienceHub.Integrations.Noaa.Ace.Responses;

/// <summary>
/// 1-minute averaged Real-time Bulk Parameters of the Solar Wind Plasma
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="Status">Data Status. 0 = nominal data 1 to 8 = bad data record 9 = no data</param>
/// <param name="ProtonDensity">Solar Wind proton density</param>
/// <param name="BulkSpeed">Solar Wind plasma speed</param>
/// <param name="IonTemperature">Solar Wind Ion Temp</param>
public record SolarWindPlasmaRecord(
    DateTime DateTime,
    int Status,
    float? ProtonDensity,
    float? BulkSpeed,
    float? IonTemperature
);
