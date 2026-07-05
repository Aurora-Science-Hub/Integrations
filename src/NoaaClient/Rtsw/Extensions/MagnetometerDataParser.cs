using System.Text.Json;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;

namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw.Extensions;

internal static class MagnetometerDataParser
{
    private const string Context = "RTSW magnetometer";

    /// <summary>
    /// Parses RTSW magnetometer payload into strongly typed records.
    /// </summary>
    public static IReadOnlyList<MagnetometerRecord> Parse(string text)
    {
        using var jsonDoc = JsonDocument.Parse(text);
        var rootElement = jsonDoc.RootElement;
        if (rootElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("RTSW magnetometer payload must be a JSON array.");
        }

        if (rootElement.GetArrayLength() == 0)
        {
            return Array.Empty<MagnetometerRecord>();
        }

        var records = new List<MagnetometerRecord>(rootElement.GetArrayLength());
        foreach (var item in rootElement.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException("RTSW magnetometer item must be a JSON object.");
            }

            records.Add(new MagnetometerRecord(
                DateTime: RtswJsonFieldParser.ParseDateTime(item, "time_tag", Context),
                Active: RtswJsonFieldParser.ParseBool(item, "active", Context),
                Source: RtswJsonFieldParser.ParseRequiredString(item, "source", Context),
                Range: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "range", Context),
                Scale: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "scale", Context),
                Sensitivity: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "sensitivity", Context),
                ManualMode: RtswJsonFieldParser.ParseOptionalNullableBool(item, "manual_mode", Context),
                SampleSize: RtswJsonFieldParser.ParseOptionalNullableInt(item, "sample_size", Context),
                Bt: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "bt", Context),
                BxGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "bx_gse", Context),
                ByGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "by_gse", Context),
                BzGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "bz_gse", Context),
                ThetaGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "theta_gse", Context),
                PhiGse: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "phi_gse", Context),
                BxGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "bx_gsm", Context),
                ByGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "by_gsm", Context),
                BzGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "bz_gsm", Context),
                ThetaGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "theta_gsm", Context),
                PhiGsm: RtswJsonFieldParser.ParseOptionalNullableFloat(item, "phi_gsm", Context),
                MaxTelemetryFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_telemetry_flag", Context),
                MaxDataFlag: RtswJsonFieldParser.ParseOptionalNullableInt(item, "max_data_flag", Context),
                OverallQuality: RtswJsonFieldParser.ParseOptionalNullableInt(item, "overall_quality", Context)));
        }

        return records;
    }
}
