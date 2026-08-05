#pragma warning disable CS0618 // Sample still demonstrates deprecated IAceClient until issue #3.

using AuroraScienceHub.Integrations.NoaaClient;
using AuroraScienceHub.Integrations.NoaaClient.Ace;
using AuroraScienceHub.Integrations.NoaaClient.WsaEnlil;
using AuroraScienceHub.Integrations.NoaaClient.KpIndex;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw;
using AuroraScienceHub.Integrations.Samples.NoaaClientSample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

// Setup DI and configuration
var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
builder.Services.AddNoaaClients();
var host = builder.Build();

// Get NOAA clients from DI
var aceClient = host.Services.GetRequiredService<IAceClient>();
var kpIndexClient = host.Services.GetRequiredService<IKpIndexClient>();
var rtswClient = host.Services.GetRequiredService<IRtswClient>();
var wsaEnlilClient = host.Services.GetRequiredService<IWsaEnlilClient>();

// Display header
AnsiConsole.Write(new FigletText("NOAA Client").Color(Color.Blue));
AnsiConsole.MarkupLine("[dim]Interactive sample application for NOAA API clients[/]\n");

// Interactive menu loop
while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Select an option:[/]")
            .PageSize(10)
            .AddChoices(
                "ACE Magnetometer (deprecated)",
                "ACE SWEPAM (deprecated)",
                "RTSW Magnetometer",
                "RTSW Solar Wind Plasma",
                "KP Index 27-Day Forecast",
                "KP Index 3-Day Forecast",
                "KP Index Nowcast",
                "WSA-ENLIL Animation",
                "WSA-ENLIL Animation (small, 320px)",
                new string('-', 30),
                "Execute All Requests",
                "Exit"));

    if (choice == "Exit") break;
    if (choice.StartsWith("---")) continue;

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
                    "ACE Magnetometer (deprecated)" => FetchAndDisplay(async () =>
                    {
                        var data = await aceClient.GetMagnetometerDataAsync(CancellationToken.None);
                        OutputFormatter.DisplayAceMagnetometer(data, 10);
                    }),
                    "ACE SWEPAM (deprecated)" => FetchAndDisplay(async () =>
                    {
                        var data = await aceClient.GetSwepamDataAsync(CancellationToken.None);
                        OutputFormatter.DisplayAceSwepam(data, 10);
                    }),
                    "RTSW Magnetometer" => FetchAndDisplay(async () =>
                    {
                        var data = await rtswClient.GetMagnetometerDataAsync(CancellationToken.None);
                        OutputFormatter.DisplayRtswMagnetometer(data, 10);
                    }),
                    "RTSW Solar Wind Plasma" => FetchAndDisplay(async () =>
                    {
                        var data = await rtswClient.GetSolarWindPlasmaDataAsync(CancellationToken.None);
                        OutputFormatter.DisplayRtswPlasma(data, 10);
                    }),
                    "KP Index 27-Day Forecast" => FetchAndDisplay(async () =>
                    {
                        var data = await kpIndexClient.GetKpIndex27DayForecastAsync(CancellationToken.None);
                        OutputFormatter.DisplayKpForecast(data, 10, r => (
                            r.Date.ToString("yyyy-MM-dd"),
                            r.KpIndex.ToString(),
                            GetActivityLevel(r.KpIndex)));
                    }),
                    "KP Index 3-Day Forecast" => FetchAndDisplay(async () =>
                    {
                        var data = await kpIndexClient.GetKpIndex3DayForecastAsync(CancellationToken.None);
                        OutputFormatter.DisplayKpForecast(data, 10, r => (
                            r.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            r.KpIndex.ToString("F2"),
                            GetActivityLevel((int)r.KpIndex)));
                    }),
                    "KP Index Nowcast" => FetchAndDisplay(async () =>
                    {
                        var data = await kpIndexClient.GetKpIndexNowcastAsync(CancellationToken.None);
                        OutputFormatter.DisplayKpNowcast(data, 10);
                    }),
                    "WSA-ENLIL Animation" => FetchAndDisplay(async () =>
                    {
                        var gifBytes = await wsaEnlilClient.GetEnlilAnimationAsync(
                            maxWidth: 480, cancellationToken: CancellationToken.None);
                        await SaveAnimationAsync(gifBytes, "enlil_animation.gif");
                    }),
                    "WSA-ENLIL Animation (small, 320px)" => FetchAndDisplay(async () =>
                    {
                        var gifBytes = await wsaEnlilClient.GetEnlilAnimationAsync(
                            maxWidth: 320, cancellationToken: CancellationToken.None);
                        await SaveAnimationAsync(gifBytes, "enlil_animation_small.gif");
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
    AnsiConsole.MarkupLine("[yellow]Executing all NOAA API requests...[/]\n");

    var tasks = new (string name, Func<Task> action)[]
    {
        ("ACE Magnetometer", async () =>
        {
            var data = await aceClient.GetMagnetometerDataAsync(CancellationToken.None);
            OutputFormatter.DisplayAceMagnetometer(data, 5);
        }),
        ("ACE SWEPAM", async () =>
        {
            var data = await aceClient.GetSwepamDataAsync(CancellationToken.None);
            OutputFormatter.DisplayAceSwepam(data, 5);
        }),
        ("RTSW Magnetometer", async () =>
        {
            var data = await rtswClient.GetMagnetometerDataAsync(CancellationToken.None);
            OutputFormatter.DisplayRtswMagnetometer(data, 5);
        }),
        ("RTSW Solar Wind", async () =>
        {
            var data = await rtswClient.GetSolarWindPlasmaDataAsync(CancellationToken.None);
            OutputFormatter.DisplayRtswPlasma(data, 5);
        }),
        ("KP Index Nowcast", async () =>
        {
            var data = await kpIndexClient.GetKpIndexNowcastAsync(CancellationToken.None);
            OutputFormatter.DisplayKpNowcast(data, 5);
        }),
        ("WSA-ENLIL Animation", async () =>
        {
            var gifBytes = await wsaEnlilClient.GetEnlilAnimationAsync(
                maxWidth: 480, cancellationToken: CancellationToken.None);
            await SaveAnimationAsync(gifBytes, "enlil_animation.gif");
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

// Helper to fetch and display data
static async Task FetchAndDisplay(Func<Task> action) => await action();

// Helper method for activity level
static string GetActivityLevel(int kpIndex) => kpIndex switch
{
    <= 2 => "Low",
    <= 4 => "Moderate",
    <= 6 => "Elevated",
    <= 8 => "High",
    _ => "Extreme"
};

// Saves GIF animation bytes to a temp file and displays the result
static async Task SaveAnimationAsync(byte[] gifBytes, string fileName)
{
    if (gifBytes.Length == 0)
    {
        AnsiConsole.MarkupLine("[red]No animation data returned.[/]");
        return;
    }

    var outputPath = Path.Combine(Path.GetTempPath(), fileName);
    await File.WriteAllBytesAsync(outputPath, gifBytes, CancellationToken.None);

    AnsiConsole.MarkupLine($"[green]Animation saved:[/] {outputPath}");
    AnsiConsole.MarkupLine($"[dim]Size: {gifBytes.Length / 1024} KB | Format: GIF[/]");
}


