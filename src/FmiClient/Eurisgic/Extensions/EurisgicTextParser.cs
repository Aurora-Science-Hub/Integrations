using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AuroraScienceHub.Integrations.FmiClient.Eurisgic.Responses;

namespace AuroraScienceHub.Integrations.FmiClient.Eurisgic.Extensions;

// EURISGIC data example:
// 2026 11 04 15 15 00     -4.2     -8.0  10
internal static class EurisgicTextParser
{
    private const int FieldCount = 9;

    public static IReadOnlyList<EurisgicRecord> Parse(string text)
    {
        var records = new List<EurisgicRecord>();
        foreach (var line in text.AsSpan().EnumerateLines())
        {
            if (TryParseLine(line, out var record))
            {
                records.Add(record);
            }
        }

        return records;
    }

    private static bool TryParseLine(ReadOnlySpan<char> line, [NotNullWhen(true)] out EurisgicRecord? record)
    {
        record = null;
        var content = line.Trim();
        if (content.IsEmpty || content.StartsWith("%", StringComparison.Ordinal))
        {
            return false;
        }

        Span<Range> fields = stackalloc Range[FieldCount + 1];
        if (content.Split(fields, ' ', StringSplitOptions.RemoveEmptyEntries) != FieldCount)
        {
            return false;
        }

        if (!TryParseInt(content[fields[0]], out var year)
            || !TryParseInt(content[fields[1]], out var month)
            || !TryParseInt(content[fields[2]], out var day)
            || !TryParseInt(content[fields[3]], out var hour)
            || !TryParseInt(content[fields[4]], out var minute)
            || !TryParseInt(content[fields[5]], out var second)
            || !float.TryParse(content[fields[6]], NumberStyles.Float, CultureInfo.InvariantCulture, out var iu)
            || !float.TryParse(content[fields[7]], NumberStyles.Float, CultureInfo.InvariantCulture, out var il)
            || !TryParseInt(content[fields[8]], out var stationCount)
            || !TryCreateTimestamp(year, month, day, hour, minute, second, out var timestamp))
        {
            return false;
        }

        record = new EurisgicRecord(
            timestamp,
            float.IsNaN(iu) ? null : iu,
            float.IsNaN(il) ? null : il,
            stationCount);
        return true;
    }

    private static bool TryCreateTimestamp(
        int year,
        int month,
        int day,
        int hour,
        int minute,
        int second,
        out DateTime timestamp)
    {
        timestamp = default;
        if (year < DateTime.MinValue.Year
            || year > DateTime.MaxValue.Year
            || month is < 1 or > 12
            || day < 1
            || day > DateTime.DaysInMonth(year, month)
            || hour is < 0 or > 23
            || minute is < 0 or > 59
            || second is < 0 or > 60)
        {
            return false;
        }

        timestamp = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
        if (second == 60)
        {
            if (timestamp.Ticks > DateTime.MaxValue.Ticks - TimeSpan.TicksPerMinute)
            {
                return false;
            }

            timestamp = timestamp.AddMinutes(1);
        }
        else
        {
            timestamp = timestamp.AddSeconds(second);
        }

        return true;
    }

    private static bool TryParseInt(ReadOnlySpan<char> field, out int value)
        => int.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
}
