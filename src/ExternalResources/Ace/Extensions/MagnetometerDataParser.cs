using AuroraScienceHub.Framework.Utilities.System;
using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Ace.Responses;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Ace.Extensions;

// ACE Magnetometer data example:
// # Status(S): 0 = nominal data, 1 to 8 = bad data record, 9 = no data
// # Missing data values: -999.9
// #                 Modified Seconds
// # UT Date   Time  Julian   of the   ----------------  GSM Coordinates ---------------
// # YR MO DA  HHMM    Day      Day    S     Bx      By      Bz      Bt     Lat.   Long.
// #------------------------------------------------------------------------------------
// 2024 05 05  0846   60435   31560    0    -0.7    -0.8    -1.9     2.2   -60.5   225.8
internal static class MagnetometerDataParser
{
    private const string MissingDataValue = "-999.9";

    public static IReadOnlyList<MagnetometerRecord> Parse(string text)
    {
        var records = new List<MagnetometerRecord>();
        var lines = text.SplitLines();
        Span<Range> fieldsRange = stackalloc Range[32];

        foreach (var line in lines)
        {
            var trimmedLine = line.Line.Trim();
            if (trimmedLine.Length == 0 || trimmedLine[0] == '#')
            {
                continue;
            }
            var rangesCount = trimmedLine.Split(fieldsRange, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // check if line contains data
            if (rangesCount != 13 || !trimmedLine[fieldsRange[0]].IsParsableAsInt())
            {
                continue;
            }

            var record = new MagnetometerRecord
            (
                DateTime: new DateTime(
                    year: trimmedLine[fieldsRange[0]].ParseIntInvariant(),
                    month: trimmedLine[fieldsRange[1]].ParseIntInvariant(),
                    day: trimmedLine[fieldsRange[2]].ParseIntInvariant(),
                    hour: trimmedLine[fieldsRange[3]][..2].ParseIntInvariant(),
                    minute: trimmedLine[fieldsRange[3]][2..].ParseIntInvariant(),
                    second: 0,
                    DateTimeKind.Utc),
                Status: trimmedLine[fieldsRange[6]].ParseIntInvariant(),
                Bx: GetFloatOrNull(trimmedLine[fieldsRange[7]]),
                By: GetFloatOrNull(trimmedLine[fieldsRange[8]]),
                Bz: GetFloatOrNull(trimmedLine[fieldsRange[9]]),
                Bt: GetFloatOrNull(trimmedLine[fieldsRange[10]]),
                Latitude: GetFloatOrNull(trimmedLine[fieldsRange[11]]),
                Longitude: GetFloatOrNull(trimmedLine[fieldsRange[12]])
            );
            records.Add(record);
        }

        return records;
    }

    private static float? GetFloatOrNull(ReadOnlySpan<char> value)
        => value.SequenceEqual(MissingDataValue)
            ? null
            : value.ParseFloatInvariant();
}
