using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Responses;
using Spectre.Console;

namespace AuroraScienceHub.Integrations.Samples.DtuSpaceClientSample;

/// <summary>
/// Formats DTU Space data for console output using Spectre.Console
/// </summary>
internal static class OutputFormatter
{
    /// <summary>
    /// Displays PC index data in table format with a summary
    /// </summary>
    public static void DisplayPcIndex(string title, IReadOnlyList<PcIndexRecord> data, int limit)
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
            .AddColumn("[cyan]PC (mV/m)[/]");

        foreach (var record in data.TakeLast(limit))
        {
            table.AddRow(
                record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Format(record.Value));
        }

        AnsiConsole.Write(table);
        DisplaySummary(data);
    }

    private static void DisplaySummary(IReadOnlyList<PcIndexRecord> data)
    {
        var values = data.Where(r => r.Value.HasValue).Select(r => r.Value!.Value).ToList();
        var missingCount = data.Count - values.Count;

        var summary = values.Count == 0
            ? "[dim]all values are missing[/]"
            : $"min [cyan]{values.Min():F2}[/] | max [cyan]{values.Max():F2}[/] | latest [cyan]{Format(data.LastOrDefault(r => r.Value.HasValue)?.Value)}[/]";

        AnsiConsole.MarkupLine(
            $"[dim]Period:[/] {data[0].DateTime:yyyy-MM-dd HH:mm} – {data[^1].DateTime:yyyy-MM-dd HH:mm} UTC | {summary} | missing [cyan]{missingCount}[/]");
    }

    // Helper methods
    private static string Format(float? value) => value?.ToString("F2") ?? "[dim]N/A[/]";
}
