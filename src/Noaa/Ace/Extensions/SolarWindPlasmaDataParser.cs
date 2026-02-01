using AuroraScienceHub.Framework.Utilities.System;
using AuroraScienceHub.Integrations.Noaa.Ace.Responses;

namespace AuroraScienceHub.Integrations.Noaa.Ace.Extensions;

// #   1-minute averaged Real-time Bulk Parameters of the Solar Wind Plasma
// # Status(S): 0 = nominal data, 1 to 8 = bad data record, 9 = no data
// # Missing data values: Density and Speed = -9999.9, Temp. = -1.00e+05
// #                Modified Seconds   -------------  Solar Wind  -----------
// # UT Date   Time  Julian  of the          Proton      Bulk         Ion
// # YR MO DA  HHMM    Day     Day     S    Density     Speed     Temperature
// #-------------------------------------------------------------------------
// 2024 05 06  1657   60436   61020    1        4.9      475.0     2.38e+05
internal static class SolarWindPlasmaDataParser
{
    private const string MissingDataValue = "-9999.9";
    private const string MissingTemperatureValue = "-1.00e+05";

    public static IReadOnlyList<SolarWindPlasmaRecord> Parse(string text)
    {
        var records = new List<SolarWindPlasmaRecord>();
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
            if (rangesCount != 10 || !trimmedLine[fieldsRange[0]].IsParsableAsInt())
            {
                continue;
            }

            var record = new SolarWindPlasmaRecord
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
                ProtonDensity: GetFloatOrNull(trimmedLine[fieldsRange[7]]),
                BulkSpeed: GetFloatOrNull(trimmedLine[fieldsRange[8]]),
                IonTemperature: GetTemperatureOrNull(trimmedLine[fieldsRange[9]])
            );
            records.Add(record);
        }

        return records;
    }

    private static float? GetFloatOrNull(ReadOnlySpan<char> value)
        => value.SequenceEqual(MissingDataValue)
            ? null
            : value.ParseFloatInvariant();

    private static float? GetTemperatureOrNull(ReadOnlySpan<char> value)
        => value.SequenceEqual(MissingTemperatureValue)
            ? null
            : value.ParseFloatInvariant();
}
