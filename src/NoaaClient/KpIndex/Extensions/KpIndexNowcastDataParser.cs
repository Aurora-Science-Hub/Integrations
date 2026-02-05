using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using AuroraScienceHub.Integrations.NoaaClient.KpIndex.Responses;

namespace AuroraScienceHub.Integrations.NoaaClient.KpIndex.Extensions;

// Current KP-index forecast data example:
// [
//     [
//         "time_tag",
//         "Kp",
//         "a_running",
//         "station_count"
//     ],
//     [
//         "2025-01-05 00:00:00.000",
//         "3.67",
//         "22",
//         "8"
//     ]
// ]

internal static class KpIndexNowcastDataParser
{
    public static IReadOnlyList<KpIndexNowcastResponse> Parse(string text)
    {
        using var jsonDocument = JsonDocument.Parse(text);
        var rootElement = jsonDocument.RootElement;

        if (rootElement.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<KpIndexNowcastResponse>();
        }

        var arrayLength = rootElement.GetArrayLength();
        if (arrayLength < 2)
        {
            return Array.Empty<KpIndexNowcastResponse>();
        }

        var responses = new List<KpIndexNowcastResponse>(arrayLength);
        foreach (var jsonElement in rootElement.EnumerateArray().Skip(1))
        {
            if (TryParseElement(jsonElement, out var response))
            {
                responses.Add(response);
            }
        }

        return responses;
    }

    private static bool TryParseElement(JsonElement jsonElement, [NotNullWhen(true)] out KpIndexNowcastResponse? response)
    {
        if (jsonElement.ValueKind != JsonValueKind.Array ||
            jsonElement.GetArrayLength() < 4)
        {
            response = null;
            return false;
        }

        var dateTimeString = jsonElement[0].GetString();
        if (!DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dateTime))
        {
            response = null;
            return false;
        }

        var kpIndexString = jsonElement[1].GetString();
        if (!float.TryParse(kpIndexString, NumberFormatInfo.InvariantInfo, out var kpIndex))
        {
            response = null;
            return false;
        }

        var stationsCountString = jsonElement[3].GetString();
        if (!int.TryParse(stationsCountString, NumberFormatInfo.InvariantInfo, out var stationsCount))
        {
            response = null;
            return false;
        }

        response = new KpIndexNowcastResponse(dateTime, kpIndex, stationsCount);
        return true;
    }
}
