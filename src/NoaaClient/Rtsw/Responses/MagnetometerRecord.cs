namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;

/// <summary>
/// 1-minute averaged real-time interplanetary magnetic field values (RTSW).
/// </summary>
/// <param name="DateTime">Date and time of record (UTC)</param>
/// <param name="Active">Whether this source is the active feed for the timestamp</param>
/// <param name="Source">Data source identifier reported by NOAA</param>
/// <param name="Range">Instrument range metadata</param>
/// <param name="Scale">Instrument scale metadata</param>
/// <param name="Sensitivity">Instrument sensitivity metadata</param>
/// <param name="ManualMode">Manual mode flag reported by NOAA</param>
/// <param name="SampleSize">Number of samples averaged in the record</param>
/// <param name="Bt">Total magnetic field magnitude</param>
/// <param name="BxGse">IMF GSE x-component</param>
/// <param name="ByGse">IMF GSE y-component</param>
/// <param name="BzGse">IMF GSE z-component</param>
/// <param name="ThetaGse">IMF GSE theta angle</param>
/// <param name="PhiGse">IMF GSE phi angle</param>
/// <param name="BxGsm">IMF GSM x-component</param>
/// <param name="ByGsm">IMF GSM y-component</param>
/// <param name="BzGsm">IMF GSM z-component</param>
/// <param name="ThetaGsm">IMF GSM theta angle</param>
/// <param name="PhiGsm">IMF GSM phi angle</param>
/// <param name="MaxTelemetryFlag">Maximum telemetry quality flag</param>
/// <param name="MaxDataFlag">Maximum data quality flag</param>
/// <param name="OverallQuality">Overall quality indicator</param>
public record MagnetometerRecord(
    DateTime DateTime,
    bool Active,
    string Source,
    float? Range,
    float? Scale,
    float? Sensitivity,
    bool? ManualMode,
    int? SampleSize,
    float? Bt,
    float? BxGse,
    float? ByGse,
    float? BzGse,
    float? ThetaGse,
    float? PhiGse,
    float? BxGsm,
    float? ByGsm,
    float? BzGsm,
    float? ThetaGsm,
    float? PhiGsm,
    int? MaxTelemetryFlag,
    int? MaxDataFlag,
    int? OverallQuality);
