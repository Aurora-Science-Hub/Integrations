using System.Globalization;
using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;

namespace AuroraScienceHub.Integrations.WdcClient.DstIndex.Extensions;

internal static class DstIndexWdcParser
{
    private const int RecordLength = 120;
    private const string IndexName = "DST";
    private const int IndexMarkerOffset = 7;
    private const char IndexMarker = '*';
    private const int ValueTypeOffset = 12;
    private const char ValueType = 'X';
    private const int HourlyValuesOffset = 20;
    private const int ValueWidth = 4;
    private const int HoursPerDay = 24;
    private const int MissingValue = 9999;
    private const int BaseValueScale = 100;
    private const int LegacyCentury = 19;
    private const int YearsPerCentury = 100;

    private static readonly Range s_yearField = 3..5;
    private static readonly Range s_monthField = 5..7;
    private static readonly Range s_dayField = 8..10;
    private static readonly Range s_centuryField = 14..16;
    private static readonly Range s_baseValueField = 16..20;

    public static IReadOnlyList<DstIndexRecord> Parse(string text)
    {
        var records = new List<DstIndexRecord>();
        foreach (var line in text.AsSpan().EnumerateLines())
        {
            if (!TryParseHeader(line, out var date, out var baseValue))
            {
                continue;
            }

            for (var hour = 0; hour < HoursPerDay; hour++)
            {
                var valueField = line.Slice(HourlyValuesOffset + hour * ValueWidth, ValueWidth);
                if (!TryParseInt(valueField, out var value))
                {
                    continue;
                }

                int? dst = value == MissingValue ? null : baseValue * BaseValueScale + value;
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
        date = default;
        baseValue = default;
        if (!IsDstRecord(line))
        {
            return false;
        }

        return TryParseDate(line, out date)
            && TryParseInt(line[s_baseValueField], out baseValue);
    }

    private static bool IsDstRecord(ReadOnlySpan<char> line)
        => line.Length >= RecordLength
           && line.StartsWith(IndexName, StringComparison.Ordinal)
           && line[IndexMarkerOffset] == IndexMarker
           && line[ValueTypeOffset] == ValueType;

    private static bool TryParseDate(ReadOnlySpan<char> line, out DateTime date)
    {
        date = default;
        if (!TryParseInt(line[s_yearField], out var year)
            || !TryParseInt(line[s_monthField], out var month)
            || !TryParseInt(line[s_dayField], out var day))
        {
            return false;
        }

        var centuryField = line[s_centuryField];
        var century = LegacyCentury;
        if (!centuryField.IsWhiteSpace()
            && !TryParseInt(centuryField, out century))
        {
            return false;
        }

        year += century * YearsPerCentury;
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
