using AuroraScienceHub.Integrations.DtuSpaceClient;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Responses;
using AuroraScienceHub.Integrations.Samples.DtuSpaceClientSample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

// Setup DI and configuration
var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
builder.Services.AddDtuSpaceClients(builder.Configuration);
var host = builder.Build();

// Get DTU Space clients from DI
var pcIndexClient = host.Services.GetRequiredService<IPcIndexClient>();
var timeProvider = TimeProvider.System;

// Display header
AnsiConsole.Write(new FigletText("DTU Space").Color(Color.Blue));
AnsiConsole.MarkupLine("[dim]Interactive sample application for DTU Space clients[/]\n");

// Interactive menu loop
while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Select an option:[/]")
            .PageSize(10)
            .AddChoices(
                "PCN Index Today",
                "PCN Index Yesterday",
                "PCN Index Last 24 Hours",
                "PCN Index Date Range",
                new string('-', 30),
                "Execute All Requests",
                "Exit"));

    if (choice == "Exit")
    {
        break;
    }

    if (choice.StartsWith("---"))
    {
        continue;
    }

    // Input prompts cannot run inside the status spinner, so the range is requested beforehand
    var range = choice == "PCN Index Date Range" ? PromptDateRange() : default;

    AnsiConsole.WriteLine();

    try
    {
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("green"))
            .StartAsync("Fetching data...", async ctx =>
            {
                await (choice switch
                {
                    "PCN Index Today" => FetchAndDisplay(async () =>
                    {
                        var data = await pcIndexClient.GetPcnAsync(GetUtcToday(), CancellationToken.None);
                        OutputFormatter.DisplayPcIndex("PCN Index Today", data, 10);
                    }),
                    "PCN Index Yesterday" => FetchAndDisplay(async () =>
                    {
                        var data = await pcIndexClient.GetPcnAsync(GetUtcToday().AddDays(-1), CancellationToken.None);
                        OutputFormatter.DisplayPcIndex("PCN Index Yesterday", data, 10);
                    }),
                    "PCN Index Last 24 Hours" => FetchAndDisplay(async () =>
                    {
                        var data = await GetPcnLast24HoursAsync();
                        OutputFormatter.DisplayPcIndex("PCN Index Last 24 Hours", data, 10);
                    }),
                    "PCN Index Date Range" => FetchAndDisplay(async () =>
                    {
                        var data = await pcIndexClient.GetPcnAsync(range.From, range.To, CancellationToken.None);
                        OutputFormatter.DisplayPcIndex($"PCN Index {range.From:yyyy-MM-dd} – {range.To:yyyy-MM-dd}", data, 10);
                    }),
                    "Execute All Requests" => ExecuteAllRequests(),
                    _ => Task.CompletedTask
                });
            });
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
    }

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
    Console.ReadKey(true);
    AnsiConsole.Clear();
}

AnsiConsole.MarkupLine("[blue]Goodbye![/]");
return 0;

async Task ExecuteAllRequests()
{
    AnsiConsole.MarkupLine("[yellow]Executing all DTU Space requests...[/]\n");

    var tasks = new (string name, Func<Task> action)[]
    {
        ("PCN Index Today", async () =>
        {
            var data = await pcIndexClient.GetPcnAsync(GetUtcToday(), CancellationToken.None);
            OutputFormatter.DisplayPcIndex("PCN Index Today", data, 5);
        }),
        ("PCN Index Yesterday", async () =>
        {
            var data = await pcIndexClient.GetPcnAsync(GetUtcToday().AddDays(-1), CancellationToken.None);
            OutputFormatter.DisplayPcIndex("PCN Index Yesterday", data, 5);
        }),
        ("PCN Index Last 24 Hours", async () =>
        {
            var data = await GetPcnLast24HoursAsync();
            OutputFormatter.DisplayPcIndex("PCN Index Last 24 Hours", data, 5);
        })
    };

    await AnsiConsole.Progress()
        .AutoClear(false)
        .Columns(
            new TaskDescriptionColumn(),
            new ProgressBarColumn(),
            new PercentageColumn(),
            new SpinnerColumn())
        .StartAsync(async ctx =>
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                var progressTask = ctx.AddTask($"[green]{tasks[i].name}[/]");
                await tasks[i].action();
                progressTask.Increment(100);
                AnsiConsole.WriteLine();
            }
        });

    AnsiConsole.MarkupLine("\n[green]✓ All requests completed successfully![/]");
}

// The last 24 hours span two UTC day files, so both days are requested and trimmed to the window
async Task<IReadOnlyList<PcIndexRecord>> GetPcnLast24HoursAsync()
{
    var now = timeProvider.GetUtcNow().UtcDateTime;
    var today = DateOnly.FromDateTime(now);
    var data = await pcIndexClient.GetPcnAsync(today.AddDays(-1), today, CancellationToken.None);
    return data.Where(r => r.DateTime > now.AddHours(-24)).ToList();
}

DateOnly GetUtcToday() => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

(DateOnly From, DateOnly To) PromptDateRange()
{
    var today = GetUtcToday();
    var from = AnsiConsole.Prompt(
        new TextPrompt<DateOnly>("[green]From (UTC, yyyy-MM-dd):[/]")
            .DefaultValue(today.AddDays(-2)));
    var to = AnsiConsole.Prompt(
        new TextPrompt<DateOnly>("[green]To (UTC, yyyy-MM-dd):[/]")
            .DefaultValue(today));
    return (from, to);
}

// Helper to fetch and display data
static async Task FetchAndDisplay(Func<Task> action) => await action();
