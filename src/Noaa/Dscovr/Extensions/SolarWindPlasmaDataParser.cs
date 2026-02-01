using System.Text.Json;
using AuroraScienceHub.Framework.Utilities.System;
using AuroraScienceHub.Integrations.Noaa.Dscovr.Responses;

namespace AuroraScienceHub.Integrations.Noaa.Dscovr.Extensions;

// DSCOVR Solar wind plasma data example:
// [
//   [
//     "time_tag",
//     "density",
//     "speed",
//     "temperature"
//   ],
//   [
//     "2024-05-20 05:51:00.000",
//     "2.18",
//     "393.9",
//     "36967"
//   ],
//   [
//     "2024-05-20 05:52:00.000",
//     "2.60",
//     "397.4",
//     "44618"
//   ]
// ]
internal static class SolarWindPlasmaDataParser
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    public static IReadOnlyList<SolarWindPlasmaRecord> Parse(string text)
    {
        using var jsonDoc = JsonDocument.Parse(text);
        var rootElement = jsonDoc.RootElement;

        if (rootElement.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<SolarWindPlasmaRecord>();
        }

        var arrayLength = rootElement.GetArrayLength();
        if (rootElement.GetArrayLength() < 2)
        {
            return Array.Empty<SolarWindPlasmaRecord>();
        }

        var fieldNames = rootElement[0];
        var solarWindPlasmaRecords = new List<SolarWindPlasmaRecord>(arrayLength);

        var timeTagIndex = GetIndex(fieldNames, "time_tag");
        var densityIndex = GetIndex(fieldNames, "density");
        var speedIndex = GetIndex(fieldNames, "speed");
        var temperatureIndex = GetIndex(fieldNames, "temperature");

        foreach (var record in rootElement.EnumerateArray().Skip(1))
        {
            solarWindPlasmaRecords.Add(new SolarWindPlasmaRecord(
                DateTime: DateTime.ParseExact(
                    record[timeTagIndex].GetString().Required(), DateTimeFormat,
                    provider: null,
                    style: System.Globalization.DateTimeStyles.AssumeUniversal |
                           System.Globalization.DateTimeStyles.AdjustToUniversal),
                ProtonDensity: record[densityIndex].GetString()?.ParseFloatInvariant(),
                BulkSpeed: record[speedIndex].GetString()?.ParseFloatInvariant(),
                IonTemperature: record[temperatureIndex].GetString()?.ParseFloatInvariant()
            ));
        }

        return solarWindPlasmaRecords;
    }

    private static int GetIndex(JsonElement fieldNames, string fieldName)
    {
        var index = 0;
        foreach (var name in fieldNames.EnumerateArray())
        {
            if (string.Equals(name.GetString(), fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
            index++;
        }

        throw new InvalidOperationException($"Field '{fieldName}' not found.");
    }
}
