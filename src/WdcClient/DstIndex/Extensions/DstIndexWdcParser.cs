using System.Globalization;
using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;

namespace AuroraScienceHub.Integrations.WdcClient.DstIndex.Extensions;

internal static class DstIndexWdcParser
{
    private const int RecordLength = 120;
    private const int HourlyValuesOffset = 20;
    private const int ValueWidth = 4;
    private const int HoursPerDay = 24;
    private const int MissingValue = 9999;
    private const int BaseValueScale = 100;

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
                if (TryParseInt(line.Slice(HourlyValuesOffset + hour * ValueWidth, ValueWidth), out var value))
                {
                    records.Add(new DstIndexRecord(date.AddHours(hour), value == MissingValue ? null : baseValue * BaseValueScale + value));
                }
            }
        }

        records.Sort((left, right) => left.DateTime.CompareTo(right.DateTime));
        return records;
    }

    private static bool TryParseHeader(ReadOnlySpan<char> line, out DateTime date, out int baseValue)
    {
        date = default;
        baseValue = default;
        if (line.Length < RecordLength || !line.StartsWith("DST", StringComparison.Ordinal)
            || line[7] != '*' || line[12] != 'X'
            || !TryParseInt(line.Slice(3, 2), out var year)
            || !TryParseInt(line.Slice(5, 2), out var month)
            || !TryParseInt(line.Slice(8, 2), out var day)
            || !TryParseInt(line.Slice(16, ValueWidth), out baseValue))
        {
            return false;
        }

        var centuryField = line.Slice(14, 2);
        var century = 19;
        if (!centuryField.IsWhiteSpace() && !TryParseInt(centuryField, out century))
        {
            return false;
        }

        year += century * 100;
        if (year is < 1 or > 9999 || month is < 1 or > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
        {
            return false;
        }

        date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        return true;
    }

    private static bool TryParseInt(ReadOnlySpan<char> field, out int value)
        => int.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
}
