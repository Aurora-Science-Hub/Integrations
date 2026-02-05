using System.Text.Json;
using AuroraScienceHub.Framework.Utilities.System;
using AuroraScienceHub.Integrations.NoaaClient.Dscovr.Responses;

namespace AuroraScienceHub.Integrations.NoaaClient.Dscovr.Extensions;

// DSCOVR Magnetometer data example:
// [
//   [
//     "time_tag",
//     "bx_gsm",
//     "by_gsm",
//     "bz_gsm",
//     "lon_gsm",
//     "lat_gsm",
//     "bt"
//   ],
//   [
//     "2024-05-19 16:50:00.000",
//     "-2.25",
//     "8.71",
//     "-1.79",
//     "104.50",
//     "-11.25",
//     "9.17"
//   ]
// ]
internal static class MagnetometerDataParser
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    public static IReadOnlyList<MagnetometerRecord> Parse(string text)
    {
        using var jsonDoc = JsonDocument.Parse(text);
        var rootElement = jsonDoc.RootElement;

        if (rootElement.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<MagnetometerRecord>();
        }

        var arrayLength = rootElement.GetArrayLength();
        if (rootElement.GetArrayLength() < 2)
        {
            return Array.Empty<MagnetometerRecord>();
        }

        var fieldNames = rootElement[0];
        var magnetometerRecords = new List<MagnetometerRecord>(arrayLength);

        var timeTagIndex = GetIndex(fieldNames, "time_tag");
        var bxGsmIndex = GetIndex(fieldNames, "bx_gsm");
        var byGsmIndex = GetIndex(fieldNames, "by_gsm");
        var bzGsmIndex = GetIndex(fieldNames, "bz_gsm");
        var btIndex = GetIndex(fieldNames, "bt");
        var latGsmIndex = GetIndex(fieldNames, "lat_gsm");
        var lonGsmIndex = GetIndex(fieldNames, "lon_gsm");

        foreach (var record in rootElement.EnumerateArray().Skip(1))
        {
            magnetometerRecords.Add(new MagnetometerRecord(
                DateTime: DateTime.ParseExact(
                    record[timeTagIndex].GetString().Required(), DateTimeFormat,
                    provider: null,
                    style: System.Globalization.DateTimeStyles.AssumeUniversal |
                           System.Globalization.DateTimeStyles.AdjustToUniversal),
                Bx: record[bxGsmIndex].GetString()?.ParseFloatInvariant(),
                By: record[byGsmIndex].GetString()?.ParseFloatInvariant(),
                Bz: record[bzGsmIndex].GetString()?.ParseFloatInvariant(),
                Bt: record[btIndex].GetString()?.ParseFloatInvariant(),
                Latitude: record[latGsmIndex].GetString()?.ParseFloatInvariant(),
                Longitude: record[lonGsmIndex].GetString()?.ParseFloatInvariant()
            ));
        }

        return magnetometerRecords;
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
