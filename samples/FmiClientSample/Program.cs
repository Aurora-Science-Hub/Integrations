using AuroraScienceHub.Integrations.FmiClient;
using AuroraScienceHub.Integrations.FmiClient.Eurisgic;
using AuroraScienceHub.Integrations.Samples.FmiClientSample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
// The interactive console owns rendering, including request failures.
builder.Logging.ClearProviders();
builder.Services.AddFmiClients(builder.Configuration);
using var host = builder.Build();
var client = host.Services.GetRequiredService<IEurisgicClient>();

AnsiConsole.Write(new FigletText("FMI").Color(Color.Blue));
AnsiConsole.MarkupLine("[dim]Finnish Meteorological Institute: EURISGIC real-time IU and IL indices[/]");

while (true)
{
    var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("[green]Select an option:[/]")
        .AddChoices("EURISGIC Latest 24 Hours", "Execute All Requests", "Exit"));
    if (choice == "Exit")
    {
        break;
    }

    try
    {
        AnsiConsole.MarkupLine("[dim]Fetching data...[/]");
        var data = await client.GetIndicesAsync(CancellationToken.None);
        OutputFormatter.DisplayIndices("EURISGIC Latest 24 Hours", data, 10);
    }
    catch (Exception exception)
    {
        AnsiConsole.MarkupLine($"[red]Error: {exception.Message.EscapeMarkup()}[/]");
    }

    AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
    Console.ReadKey(true);
    AnsiConsole.Clear();
}
