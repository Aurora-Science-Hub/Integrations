using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;
using Spectre.Console;

namespace AuroraScienceHub.Integrations.Samples.WdcClientSample;

/// <summary>
/// Formats World Data Center for Geomagnetism, Kyoto data for console output using Spectre.Console
/// </summary>
internal static class OutputFormatter
{
    /// <summary>
    /// Displays Dst index data in table format with a summary
    /// </summary>
    public static void DisplayDstIndex(string title, IReadOnlyList<DstIndexRecord> data, int limit)
    {
        if (data.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]{title.EscapeMarkup()}:[/] no data published for the requested period.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]{title.EscapeMarkup()}[/] [dim](showing {Math.Min(limit, data.Count)} of {data.Count})[/]")
            .AddColumn("[cyan]DateTime (UTC)[/]")
            .AddColumn("[cyan]Dst (nT)[/]");

        foreach (var record in data.TakeLast(limit))
        {
            table.AddRow(
                record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Format(record.Value));
        }

        AnsiConsole.Write(table);
        DisplaySummary(data);
    }

    private static void DisplaySummary(IReadOnlyList<DstIndexRecord> data)
    {
        var values = data.Where(r => r.Value.HasValue).Select(r => r.Value.GetValueOrDefault()).ToList();
        var missingCount = data.Count - values.Count;

        var latest = data.LastOrDefault(record => record.Value.HasValue);
        var summary = values.Count == 0
            ? "[dim]all values are missing[/]"
            : $"min [cyan]{values.Min():F0}[/] | max [cyan]{values.Max():F0}[/] | latest [cyan]{Format(latest?.Value)}[/] at {latest?.DateTime:yyyy-MM-dd HH:mm} UTC";

        AnsiConsole.MarkupLine(
            $"[dim]Period:[/] {data[0].DateTime:yyyy-MM-dd HH:mm} – {data[^1].DateTime:yyyy-MM-dd HH:mm} UTC | {summary} | missing [cyan]{missingCount}[/]");
    }

    // Helper methods
    private static string Format(int? value) => value?.ToString("F0") ?? "[dim]N/A[/]";
}
