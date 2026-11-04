using System.Globalization;
using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;

namespace AuroraScienceHub.Integrations.WdcClient.DstIndex.Extensions;

/// <summary>
/// Parser for fixed-width WDC Dst records
/// </summary>
/// <seealso href="https://wdc.kugi.kyoto-u.ac.jp/dstae/format/dstformat.html">WDC Dst record format</seealso>
/// <seealso href="../../References/DstRecordFormat-2026-11-04.md">Local format snapshot, retrieved on 2026-11-04</seealso>
internal static class DstIndexWdcParser
{
    private const int HourlyValuesOffset = 20;

    public static IReadOnlyList<DstIndexRecord> Parse(string text)
    {
        const int valueWidth = 4;
        const int hoursPerDay = 24;
        const int missingValue = 9999;
        const int baseValueScale = 100;

        var records = new List<DstIndexRecord>();
        foreach (var line in text.AsSpan().EnumerateLines())
        {
            if (!TryParseHeader(line, out var date, out var baseValue))
            {
                continue;
            }

            for (var hour = 0; hour < hoursPerDay; hour++)
            {
                // Each hour occupies four consecutive characters after the 20-character header (WDC columns 21-116).
                var valueField = line.Slice(HourlyValuesOffset + hour * valueWidth, valueWidth);
                if (!TryParseInt(valueField, out var value))
                {
                    continue;
                }

                int? dst = value == missingValue ? null : baseValue * baseValueScale + value;
                records.Add(new DstIndexRecord(date.AddHours(hour), dst));
            }
        }

        records.Sort((left, right) => left.DateTime.CompareTo(right.DateTime));
        return records;
    }

    private static bool TryParseHeader(
        ReadOnlySpan<char> line,
        out DateTime date,
        out int baseValue)
    {
        var baseValueField = 16..HourlyValuesOffset;
        date = DateTime.MinValue;
        baseValue = 0;
        if (!IsDstRecord(line))
        {
            return false;
        }

        return TryParseDate(line, out date)
            && TryParseInt(line[baseValueField], out baseValue);
    }

    private static bool IsDstRecord(ReadOnlySpan<char> line)
    {
        const int recordLength = 120;
        const string indexName = "DST";
        const int indexMarkerOffset = 7;
        const char indexMarker = '*';
        const int valueTypeOffset = 12;
        const char valueType = 'X';

        return line.Length >= recordLength
            && line.StartsWith(indexName, StringComparison.Ordinal)
            && line[indexMarkerOffset] == indexMarker
            && line[valueTypeOffset] == valueType;
    }

    private static bool TryParseDate(ReadOnlySpan<char> line, out DateTime date)
    {
        const int legacyCentury = 19;
        const int yearsPerCentury = 100;
        var yearField = 3..5;
        var monthField = 5..7;
        var dayField = 8..10;
        var centuryField = 14..16;

        date = DateTime.MinValue;
        if (!TryParseInt(line[yearField], out var year)
            || !TryParseInt(line[monthField], out var month)
            || !TryParseInt(line[dayField], out var day))
        {
            return false;
        }

        var centuryValue = line[centuryField];
        var century = legacyCentury;
        if (!centuryValue.IsWhiteSpace()
            && !TryParseInt(centuryValue, out century))
        {
            return false;
        }

        year += century * yearsPerCentury;
        if (year < DateTime.MinValue.Year
            || year > DateTime.MaxValue.Year
            || month is < 1 or > 12
            || day < 1
            || day > DateTime.DaysInMonth(year, month))
        {
            return false;
        }

        date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        return true;
    }

    private static bool TryParseInt(ReadOnlySpan<char> field, out int value)
        => int.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
}
