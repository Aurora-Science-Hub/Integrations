using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.KpIndex.Responses;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.KpIndex.Extensions;

// 3-day KP-index forecast data example:
// [
//     [
//         "time_tag",
//         "kp",
//         "observed",
//         "noaa_scale"
//     ],
//     [
//         "2025-01-04 00:00:00",
//         "2.67",
//         "observed",
//         null
//     ]
// ]

internal static class KpIndex3DayDataParser
{
    public static IReadOnlyList<KpIndex3DayResponse> Parse(string text)
    {
        using var jsonDocument = JsonDocument.Parse(text);
        var rootElement = jsonDocument.RootElement;

        var arrayLength = rootElement.GetArrayLength();
        if (rootElement.ValueKind != JsonValueKind.Array || arrayLength < 2)
        {
            return Array.Empty<KpIndex3DayResponse>();
        }

        var responses = new List<KpIndex3DayResponse>(arrayLength);
        foreach (var jsonElement in rootElement.EnumerateArray().Skip(1))
        {
            if (TryParseElement(jsonElement, out var response))
            {
                responses.Add(response);
            }
        }

        return responses;
    }

    private static bool TryParseElement(JsonElement jsonElement, [NotNullWhen(true)] out KpIndex3DayResponse? response)
    {
        if (jsonElement.ValueKind != JsonValueKind.Array ||
            jsonElement.GetArrayLength() < 2)
        {
            response = null;
            return false;
        }

        var dateTimeString = jsonElement[0].GetString();
        if (!DateTime.TryParse(dateTimeString, out var dateTime))
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

        response = new KpIndex3DayResponse(dateTime, kpIndex);
        return true;
    }
}
