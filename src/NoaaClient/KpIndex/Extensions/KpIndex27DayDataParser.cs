using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AuroraScienceHub.Framework.Utilities.System;
using AuroraScienceHub.Integrations.NoaaClient.KpIndex.Responses;

namespace AuroraScienceHub.Integrations.NoaaClient.KpIndex.Extensions;

// 27-day KP-index forecast data example:
// :Product: 27-day Space Weather Outlook Table 27DO.txt
// :Issued: 2025 Jan 06 0242 UTC
// # Prepared by the US Dept. of Commerce, NOAA, Space Weather Prediction Center
// # Product description and SWPC contact on the Web
// # https://www.swpc.noaa.gov/content/subscription-services
// #
// #      27-day Space Weather Outlook Table
// #                Issued 2025-01-06
// #
// #   UTC      Radio Flux   Planetary   Largest
// #  Date       10.7 cm      A Index    Kp Index
// 2025 Jan 06     172          22          5
// 2025 Jan 07     165          12          4
// 2025 Jan 08     165           8          3

internal static class KpIndex27DayDataParser
{
    private const int ForecastDaysCount = 27;

    public static IReadOnlyList<KpIndex27DayResponse> Parse(string text)
    {
        var responses = new List<KpIndex27DayResponse>(ForecastDaysCount);

        var lineEntries = text.SplitLines();
        foreach (var lineEntry in lineEntries)
        {
            if (TryParseLine(lineEntry.Line, out var response))
            {
                responses.Add(response);
            }
        }

        return responses;
    }

    private static bool TryParseLine(ReadOnlySpan<char> line, [NotNullWhen(true)] out KpIndex27DayResponse? response)
    {
        var trimmedLine = line.Trim();
        if (trimmedLine.IsEmpty || trimmedLine[0] == '#' || trimmedLine[0] == ':')
        {
            response = null;
            return false;
        }

        const int fieldsInRowCount = 6;
        Span<Range> fieldsRange = stackalloc Range[fieldsInRowCount];
        var rangesCount = trimmedLine.Split(fieldsRange, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (rangesCount != fieldsInRowCount)
        {
            response = null;
            return false;
        }

        var yearString = trimmedLine[fieldsRange[0]];
        var monthString = trimmedLine[fieldsRange[1]];
        var dayString = trimmedLine[fieldsRange[2]];

        var dateString = string.Concat(yearString, monthString, dayString);
        if (!DateOnly.TryParseExact(dateString, "yyyyMMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            response = null;
            return false;
        }

        var kpIndexString = trimmedLine[fieldsRange[5]];
        if (!int.TryParse(kpIndexString, out var kpIndex))
        {
            response = null;
            return false;
        }

        response = new KpIndex27DayResponse(date, kpIndex);
        return true;
    }
}
