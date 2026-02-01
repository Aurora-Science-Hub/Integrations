using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex.Responses;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex.Extensions;

/// <summary>
/// Pc-index raw data parser
/// </summary>
internal static class PcIndexDataParser
{
    /// <summary>
    /// Parse raw Pc-index data
    /// </summary>
    /// <param name="content">Content of Pc-index raw response</param>
    /// <remarks>
    /// Example:
    /// [[1766441400000,3.2202699184417725],[1766441460000,3.098059892654419],[1766441520000,2.9881300926208496],...]]
    /// where first value is Unix time in milliseconds, second value is Pc-index.
    /// </remarks>
    public static IReadOnlyList<PcIndexResponse> Parse(string content)
    {
        using var jsonDocument = JsonDocument.Parse(content);
        var rootElement = jsonDocument.RootElement;
        var arrayLength = rootElement.GetArrayLength();

        if (rootElement.ValueKind != JsonValueKind.Array || arrayLength < 1)
        {
            return [];
        }

        var responses = new List<PcIndexResponse>(arrayLength);

        foreach (var jsonElement in rootElement.EnumerateArray())
        {
            if (TryParseElement(jsonElement, out var response))
            {
                responses.Add(response);
            }
        }

        return responses;
    }

    private static bool TryParseElement(JsonElement jsonElement, [NotNullWhen(true)] out PcIndexResponse? response)
    {
        if (jsonElement.ValueKind != JsonValueKind.Array
            || jsonElement.GetArrayLength() < 2
            || jsonElement[0].GetInt64() is 0)
        {
            response = null;
            return false;
        }

        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(jsonElement[0].GetInt64()).DateTime;

        var pcIndexRaw = jsonElement[1].GetRawText();
        float? pcIndex = float.TryParse(pcIndexRaw, NumberFormatInfo.InvariantInfo, out var pcIndexFloat)
            ? pcIndexFloat
            : null;

        response = new PcIndexResponse(dateTime, pcIndex);
        return true;
    }
}
