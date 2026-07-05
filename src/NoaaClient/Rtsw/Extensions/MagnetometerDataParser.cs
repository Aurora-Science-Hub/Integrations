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
                Bx: RtswJsonFieldParser.ParseNullableFloat(item, "bx_gsm", Context),
                By: RtswJsonFieldParser.ParseNullableFloat(item, "by_gsm", Context),
                Bz: RtswJsonFieldParser.ParseNullableFloat(item, "bz_gsm", Context),
                Bt: RtswJsonFieldParser.ParseNullableFloat(item, "bt", Context),
                Latitude: RtswJsonFieldParser.ParseNullableFloat(item, "lat_gsm", Context),
                Longitude: RtswJsonFieldParser.ParseNullableFloat(item, "lon_gsm", Context),
                Active: RtswJsonFieldParser.ParseBool(item, "active", Context),
                Source: RtswJsonFieldParser.ParseRequiredString(item, "source", Context)));
        }

        return records;
    }
}
