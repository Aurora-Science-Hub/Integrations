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
                ProtonDensity: RtswJsonFieldParser.ParseNullableFloat(item, "density", Context),
                BulkSpeed: RtswJsonFieldParser.ParseNullableFloat(item, "speed", Context),
                IonTemperature: RtswJsonFieldParser.ParseNullableFloat(item, "temperature", Context),
                Active: RtswJsonFieldParser.ParseBool(item, "active", Context),
                Source: RtswJsonFieldParser.ParseRequiredString(item, "source", Context)));
        }

        return records;
    }
}
