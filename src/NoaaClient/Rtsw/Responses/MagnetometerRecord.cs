using System.Text.Json.Serialization;

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
    [property: JsonPropertyName("time_tag")] DateTime DateTime,
    [property: JsonPropertyName("active")] bool Active,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("range")] float? Range,
    [property: JsonPropertyName("scale")] float? Scale,
    [property: JsonPropertyName("sensitivity")] float? Sensitivity,
    [property: JsonPropertyName("manual_mode")] bool? ManualMode,
    [property: JsonPropertyName("sample_size")] int? SampleSize,
    [property: JsonPropertyName("bt")] float? Bt,
    [property: JsonPropertyName("bx_gse")] float? BxGse,
    [property: JsonPropertyName("by_gse")] float? ByGse,
    [property: JsonPropertyName("bz_gse")] float? BzGse,
    [property: JsonPropertyName("theta_gse")] float? ThetaGse,
    [property: JsonPropertyName("phi_gse")] float? PhiGse,
    [property: JsonPropertyName("bx_gsm")] float? BxGsm,
    [property: JsonPropertyName("by_gsm")] float? ByGsm,
    [property: JsonPropertyName("bz_gsm")] float? BzGsm,
    [property: JsonPropertyName("theta_gsm")] float? ThetaGsm,
    [property: JsonPropertyName("phi_gsm")] float? PhiGsm,
    [property: JsonPropertyName("max_telemetry_flag")] int? MaxTelemetryFlag,
    [property: JsonPropertyName("max_data_flag")] int? MaxDataFlag,
    [property: JsonPropertyName("overall_quality")] int? OverallQuality);
