namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;

/// <summary>
/// 1-minute averaged real-time bulk parameters of the solar wind plasma (RTSW).
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="Active">Whether this source is the active feed for the timestamp</param>
/// <param name="Source">Data source identifier reported by NOAA</param>
/// <param name="ProtonSpeed">Proton bulk speed</param>
/// <param name="ProtonTemperature">Proton temperature</param>
/// <param name="ProtonDensity">Proton density</param>
/// <param name="ProtonVxGse">Proton velocity x-component in GSE</param>
/// <param name="ProtonVyGse">Proton velocity y-component in GSE</param>
/// <param name="ProtonVzGse">Proton velocity z-component in GSE</param>
/// <param name="ProtonVxGsm">Proton velocity x-component in GSM</param>
/// <param name="ProtonVyGsm">Proton velocity y-component in GSM</param>
/// <param name="ProtonVzGsm">Proton velocity z-component in GSM</param>
/// <param name="ProtonSampleSize">Proton sample size</param>
/// <param name="AlphaSpeed">Alpha particle bulk speed</param>
/// <param name="AlphaTemperature">Alpha particle temperature</param>
/// <param name="AlphaDensity">Alpha particle density</param>
/// <param name="AlphaVxGse">Alpha velocity x-component in GSE</param>
/// <param name="AlphaVyGse">Alpha velocity y-component in GSE</param>
/// <param name="AlphaVzGse">Alpha velocity z-component in GSE</param>
/// <param name="AlphaVxGsm">Alpha velocity x-component in GSM</param>
/// <param name="AlphaVyGsm">Alpha velocity y-component in GSM</param>
/// <param name="AlphaVzGsm">Alpha velocity z-component in GSM</param>
/// <param name="AlphaSampleSize">Alpha sample size</param>
/// <param name="MaxConvergenceFlag">Maximum convergence quality flag</param>
/// <param name="MaxDataFlag">Maximum data quality flag</param>
/// <param name="MaxErrorCountFlag">Maximum error count quality flag</param>
/// <param name="MaxProcessingFlag">Maximum processing quality flag</param>
/// <param name="MaxRangeFlag">Maximum range quality flag</param>
/// <param name="MaxSampleCountFlag">Maximum sample count quality flag</param>
/// <param name="MaxTelemetryFlag">Maximum telemetry quality flag</param>
/// <param name="OverallQuality">Overall quality indicator</param>
public record SolarWindPlasmaRecord(
    DateTime DateTime,
    bool Active,
    string Source,
    float? ProtonSpeed,
    float? ProtonTemperature,
    float? ProtonDensity,
    float? ProtonVxGse,
    float? ProtonVyGse,
    float? ProtonVzGse,
    float? ProtonVxGsm,
    float? ProtonVyGsm,
    float? ProtonVzGsm,
    int? ProtonSampleSize,
    float? AlphaSpeed,
    float? AlphaTemperature,
    float? AlphaDensity,
    float? AlphaVxGse,
    float? AlphaVyGse,
    float? AlphaVzGse,
    float? AlphaVxGsm,
    float? AlphaVyGsm,
    float? AlphaVzGsm,
    int? AlphaSampleSize,
    int? MaxConvergenceFlag,
    int? MaxDataFlag,
    int? MaxErrorCountFlag,
    int? MaxProcessingFlag,
    int? MaxRangeFlag,
    int? MaxSampleCountFlag,
    int? MaxTelemetryFlag,
    int? OverallQuality);
