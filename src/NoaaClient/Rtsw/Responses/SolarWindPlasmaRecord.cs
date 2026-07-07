using System.Text.Json.Serialization;

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
    [property: JsonPropertyName("time_tag")] DateTime DateTime,
    [property: JsonPropertyName("active")] bool Active,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("proton_speed")] float? ProtonSpeed,
    [property: JsonPropertyName("proton_temperature")] float? ProtonTemperature,
    [property: JsonPropertyName("proton_density")] float? ProtonDensity,
    [property: JsonPropertyName("proton_vx_gse")] float? ProtonVxGse,
    [property: JsonPropertyName("proton_vy_gse")] float? ProtonVyGse,
    [property: JsonPropertyName("proton_vz_gse")] float? ProtonVzGse,
    [property: JsonPropertyName("proton_vx_gsm")] float? ProtonVxGsm,
    [property: JsonPropertyName("proton_vy_gsm")] float? ProtonVyGsm,
    [property: JsonPropertyName("proton_vz_gsm")] float? ProtonVzGsm,
    [property: JsonPropertyName("proton_sample_size")] int? ProtonSampleSize,
    [property: JsonPropertyName("alpha_speed")] float? AlphaSpeed,
    [property: JsonPropertyName("alpha_temperature")] float? AlphaTemperature,
    [property: JsonPropertyName("alpha_density")] float? AlphaDensity,
    [property: JsonPropertyName("alpha_vx_gse")] float? AlphaVxGse,
    [property: JsonPropertyName("alpha_vy_gse")] float? AlphaVyGse,
    [property: JsonPropertyName("alpha_vz_gse")] float? AlphaVzGse,
    [property: JsonPropertyName("alpha_vx_gsm")] float? AlphaVxGsm,
    [property: JsonPropertyName("alpha_vy_gsm")] float? AlphaVyGsm,
    [property: JsonPropertyName("alpha_vz_gsm")] float? AlphaVzGsm,
    [property: JsonPropertyName("alpha_sample_size")] int? AlphaSampleSize,
    [property: JsonPropertyName("max_convergence_flag")] int? MaxConvergenceFlag,
    [property: JsonPropertyName("max_data_flag")] int? MaxDataFlag,
    [property: JsonPropertyName("max_error_count_flag")] int? MaxErrorCountFlag,
    [property: JsonPropertyName("max_processing_flag")] int? MaxProcessingFlag,
    [property: JsonPropertyName("max_range_flag")] int? MaxRangeFlag,
    [property: JsonPropertyName("max_sample_count_flag")] int? MaxSampleCountFlag,
    [property: JsonPropertyName("max_telemetry_flag")] int? MaxTelemetryFlag,
    [property: JsonPropertyName("overall_quality")] int? OverallQuality);
