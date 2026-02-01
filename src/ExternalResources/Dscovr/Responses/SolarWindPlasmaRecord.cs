namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Dscovr.Responses;

/// <summary>
/// 1-minute averaged Real-time Bulk Parameters of the Solar Wind Plasma (DSCOVR)
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="ProtonDensity">Solar Wind proton density</param>
/// <param name="BulkSpeed">Solar Wind plasma speed</param>
/// <param name="IonTemperature">Solar Wind Ion Temp</param>
public record SolarWindPlasmaRecord(
    DateTime DateTime,
    float? ProtonDensity,
    float? BulkSpeed,
    float? IonTemperature
);
