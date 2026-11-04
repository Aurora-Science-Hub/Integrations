using AuroraScienceHub.Integrations.Samples.WdcClientSample;
using AuroraScienceHub.Integrations.WdcClient;
using AuroraScienceHub.Integrations.WdcClient.DstIndex;
using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
// The interactive console owns rendering, including request failures.
builder.Logging.ClearProviders();
builder.Services.AddWdcClients(builder.Configuration);
using var host = builder.Build();
var client = host.Services.GetRequiredService<IDstIndexClient>();
var timeProvider = TimeProvider.System;

AnsiConsole.Write(new FigletText("WDC").Color(Color.Blue));
AnsiConsole.MarkupLine("[dim]World Data Center for Geomagnetism, Kyoto: real-time hourly Dst[/]");

while (true)
{
    var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("[green]Select an option:[/]")
        .AddChoices("Dst Index Today", "Dst Index Yesterday", "Dst Index Last 24 Hours",
            "Dst Index Date Range", "Execute All Requests", "Exit"));
    if (choice == "Exit")
    {
        break;
    }

    var now = timeProvider.GetUtcNow().UtcDateTime;
    var today = DateOnly.FromDateTime(now);
    try
    {
        switch (choice)
        {
            case "Dst Index Today":
                await DisplayDayAsync(choice, today, CancellationToken.None);
                break;
            case "Dst Index Yesterday":
                await DisplayDayAsync(choice, today.AddDays(-1), CancellationToken.None);
                break;
            case "Dst Index Last 24 Hours":
                await DisplayLast24HoursAsync(now, CancellationToken.None);
                break;
            case "Dst Index Date Range":
                var range = PromptDateRange(today);
                AnsiConsole.MarkupLine("[dim]Fetching data...[/]");
                var data = await client.GetDstAsync(range.From, range.To, CancellationToken.None);
                Display($"Dst Index {range.From:yyyy-MM-dd} – {range.To:yyyy-MM-dd}", data);
                break;
            case "Execute All Requests":
                await DisplayDayAsync("Dst Index Today", today, CancellationToken.None);
                await DisplayDayAsync("Dst Index Yesterday", today.AddDays(-1), CancellationToken.None);
                await DisplayLast24HoursAsync(now, CancellationToken.None);
                break;
        }
    }
    catch (Exception exception)
    {
        AnsiConsole.MarkupLine($"[red]Error: {exception.Message.EscapeMarkup()}[/]");
    }

    AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
    Console.ReadKey(true);
    AnsiConsole.Clear();
}

async Task DisplayDayAsync(
    string title,
    DateOnly date,
    CancellationToken cancellationToken)
{
    AnsiConsole.MarkupLine("[dim]Fetching data...[/]");
    var data = await client.GetDstAsync(date, cancellationToken);
    Display(title, data);
}

async Task DisplayLast24HoursAsync(DateTime now, CancellationToken cancellationToken)
{
    var today = DateOnly.FromDateTime(now);
    AnsiConsole.MarkupLine("[dim]Fetching data...[/]");
    var data = await client.GetDstAsync(today.AddDays(-1), today, cancellationToken);
    Display("Dst Index Last 24 Hours", data.Where(record => record.DateTime > now.AddHours(-24)).ToList());
}

void Display(string title, IReadOnlyList<DstIndexRecord> records)
    => OutputFormatter.DisplayDstIndex(title, SampleData.GetElapsedHours(records, timeProvider.GetUtcNow().UtcDateTime), 10);

(DateOnly From, DateOnly To) PromptDateRange(DateOnly today)
{
    var earliest = SampleData.GetEarliestDate(today);
    var message = $"Available real-time dates: {earliest:yyyy-MM-dd} through {today:yyyy-MM-dd} UTC.";
    AnsiConsole.MarkupLine($"[dim]{message} Historical dates require WDC provisional/final products.[/]");
    var from = AnsiConsole.Prompt(new TextPrompt<DateOnly>("[green]From (UTC, yyyy-MM-dd):[/]")
        .DefaultValue(today.AddDays(-2))
        .Validate(date => SampleData.IsSupportedRange(date, date, today)
            ? ValidationResult.Success() : ValidationResult.Error(message)));
    var to = AnsiConsole.Prompt(new TextPrompt<DateOnly>("[green]To (UTC, yyyy-MM-dd):[/]")
        .DefaultValue(today)
        .Validate(date => SampleData.IsSupportedRange(from, date, today)
            ? ValidationResult.Success() : ValidationResult.Error($"{message} To must be on or after From.")));
    return (from, to);
}
