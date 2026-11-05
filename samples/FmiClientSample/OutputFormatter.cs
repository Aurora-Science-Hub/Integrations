using AuroraScienceHub.Integrations.FmiClient.Eurisgic.Responses;
using Spectre.Console;

namespace AuroraScienceHub.Integrations.Samples.FmiClientSample;

/// <summary>
/// Formats FMI EURISGIC data for console output using Spectre.Console
/// </summary>
internal static class OutputFormatter
{
    /// <summary>
    /// Displays IU and IL indices in table format
    /// </summary>
    public static void DisplayIndices(string title, IReadOnlyList<EurisgicRecord> data, int limit)
    {
        if (data.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]{title.EscapeMarkup()}:[/] no data published.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]{title.EscapeMarkup()}[/] [dim](showing {Math.Min(limit, data.Count)} of {data.Count})[/]")
            .AddColumn("[cyan]DateTime (UTC)[/]")
            .AddColumn("[cyan]IU (nT)[/]")
            .AddColumn("[cyan]IL (nT)[/]")
            .AddColumn("[cyan]Stations[/]");

        foreach (var record in data.TakeLast(limit))
        {
            table.AddRow(
                record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Format(record.Iu),
                Format(record.Il),
                record.StationCount.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine(
            $"[dim]Period:[/] {data[0].DateTime:yyyy-MM-dd HH:mm} – {data[^1].DateTime:yyyy-MM-dd HH:mm} UTC");
    }

    private static string Format(float? value) => value?.ToString("F1") ?? "[dim]N/A[/]";
}
