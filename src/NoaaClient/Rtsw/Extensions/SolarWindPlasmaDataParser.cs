using System.Text.Json;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;

namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw.Extensions;

internal static class SolarWindPlasmaDataParser
{
    private const string Context = "RTSW solar wind";

    /// <summary>
    /// Parses RTSW solar wind payload into strongly typed records.
    /// </summary>
    public static IReadOnlyList<SolarWindPlasmaRecord> Parse(string text)
    {
        using var jsonDoc = JsonDocument.Parse(text);
        var rootElement = jsonDoc.RootElement;
        if (rootElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("RTSW solar wind payload must be a JSON array.");
        }

        if (rootElement.GetArrayLength() == 0)
        {
            return Array.Empty<SolarWindPlasmaRecord>();
        }

        var records = new List<SolarWindPlasmaRecord>(rootElement.GetArrayLength());
        foreach (var item in rootElement.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException("RTSW solar wind item must be a JSON object.");
            }

            records.Add(new SolarWindPlasmaRecord(
                DateTime: RtswJsonFieldParser.ParseDateTime(item, "time_tag", Context),
                Active: RtswJsonFieldParser.ParseBool(item, "active", Context),
                Source: RtswJsonFieldParser.ParseRequiredString(item, "source", Context),
                ProtonSpeed: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_speed", Context),
                ProtonTemperature: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_temperature", Context),
                ProtonDensity: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_density", Context),
                ProtonVxGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_vx_gse", Context),
                ProtonVyGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_vy_gse", Context),
                ProtonVzGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_vz_gse", Context),
                ProtonVxGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_vx_gsm", Context),
                ProtonVyGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_vy_gsm", Context),
                ProtonVzGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "proton_vz_gsm", Context),
                ProtonSampleSize: RtswJsonFieldParser.ParseOptionalNullableInt(item, "proton_sample_size", Context),
                AlphaSpeed: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_speed", Context),
                AlphaTemperature: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_temperature", Context),
                AlphaDensity: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_density", Context),
                AlphaVxGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_vx_gse", Context),
                AlphaVyGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_vy_gse", Context),
                AlphaVzGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_vz_gse", Context),
                AlphaVxGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_vx_gsm", Context),
                AlphaVyGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_vy_gsm", Context),
                AlphaVzGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "alpha_vz_gsm", Context),
                AlphaSampleSize: RtswJsonFieldParser.ParseOptionalNullableInt(item, "alpha_sample_size", Context),
                MaxConvergenceFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_convergence_flag", Context),
                MaxDataFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_data_flag", Context),
                MaxErrorCountFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_error_count_flag", Context),
                MaxProcessingFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_processing_flag", Context),
                MaxRangeFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_range_flag", Context),
                MaxSampleCountFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_sample_count_flag", Context),
                MaxTelemetryFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_telemetry_flag", Context),
                OverallQuality: RtswJsonFieldParser.ParseOptionalNullableInt(item, "overall_quality", Context)));
        }

        return records;
    }
}
