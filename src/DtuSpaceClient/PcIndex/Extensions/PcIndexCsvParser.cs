using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Responses;

namespace AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Extensions;

// PC index CSV data example:
// date,pcn
// 2026-10-17 00:00:00.000,-0.29
// 2026-10-17 00:01:00.000,NaN
internal static class PcIndexCsvParser
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    private const char Separator = ',';

    public static IReadOnlyList<PcIndexRecord> Parse(string text)
    {
        var records = new List<PcIndexRecord>();

        foreach (var line in text.AsSpan().EnumerateLines())
        {
            if (TryParseLine(line, out var record))
            {
                records.Add(record);
            }
        }

        return records;
    }

    private static bool TryParseLine(ReadOnlySpan<char> line, [NotNullWhen(true)] out PcIndexRecord? record)
    {
        record = null;

        var separatorIndex = line.IndexOf(Separator);
        if (separatorIndex < 0)
        {
            return false;
        }

        // Header line and malformed rows are skipped by the failed date parsing
        if (!DateTime.TryParseExact(
                line[..separatorIndex].Trim(),
                DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out var dateTime))
        {
            return false;
        }

        // Provisional files end each row with a trailing separator, so only the second field is taken
        var valueField = line[(separatorIndex + 1)..];
        var nextSeparatorIndex = valueField.IndexOf(Separator);
        if (nextSeparatorIndex >= 0)
        {
            valueField = valueField[..nextSeparatorIndex];
        }

        if (!float.TryParse(valueField.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            return false;
        }

        // Missing values are published as NaN
        record = new PcIndexRecord(dateTime, float.IsNaN(value) ? null : value);
        return true;
    }
}
