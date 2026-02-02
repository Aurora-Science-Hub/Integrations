using AuroraScienceHub.Integrations.Noaa.KpIndex.Responses;
using Spectre.Console;
using AceMagnetometerRecord = AuroraScienceHub.Integrations.Noaa.Ace.Responses.MagnetometerRecord;
using DscovrMagnetometerRecord = AuroraScienceHub.Integrations.Noaa.Dscovr.Responses.MagnetometerRecord;
using AceSolarWindPlasmaRecord = AuroraScienceHub.Integrations.Noaa.Ace.Responses.SolarWindPlasmaRecord;
using DscovrSolarWindPlasmaRecord = AuroraScienceHub.Integrations.Noaa.Dscovr.Responses.SolarWindPlasmaRecord;

namespace AuroraScienceHub.Integrations.Samples.NoaaClientSample;

/// <summary>
/// Formats NOAA API data for console output using Spectre.Console
/// </summary>
internal static class OutputFormatter
{
    /// <summary>
    /// Displays ACE magnetometer data in table format
    /// </summary>
    public static void DisplayAceMagnetometer(IReadOnlyList<AceMagnetometerRecord> data, int limit)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]ACE Magnetometer Data[/] [dim](showing {Math.Min(limit, data.Count)} of {data.Count})[/]")
            .AddColumn("[cyan]DateTime[/]")
            .AddColumn("[cyan]Status[/]")
            .AddColumn("[cyan]Bx (nT)[/]")
            .AddColumn("[cyan]By (nT)[/]")
            .AddColumn("[cyan]Bz (nT)[/]")
            .AddColumn("[cyan]Bt (nT)[/]")
            .AddColumn("[cyan]Lat[/]")
            .AddColumn("[cyan]Lon[/]");

        foreach (var record in data.TakeLast(limit))
        {
            table.AddRow(
                record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                record.Status.ToString(),
                Format(record.Bx),
                Format(record.By),
                Format(record.Bz),
                Format(record.Bt),
                Format(record.Latitude),
                Format(record.Longitude));
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Displays ACE SWEPAM data in table format
    /// </summary>
    public static void DisplayAceSwepam(IReadOnlyList<AceSolarWindPlasmaRecord> data, int limit)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]ACE SWEPAM Data[/] [dim](showing {Math.Min(limit, data.Count)} of {data.Count})[/]")
            .AddColumn("[cyan]DateTime[/]")
            .AddColumn("[cyan]Status[/]")
            .AddColumn("[cyan]Proton Density[/]")
            .AddColumn("[cyan]Bulk Speed[/]")
            .AddColumn("[cyan]Ion Temp[/]");

        foreach (var record in data.TakeLast(limit))
        {
            table.AddRow(
                record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                record.Status.ToString(),
                Format(record.ProtonDensity),
                Format(record.BulkSpeed),
                Format(record.IonTemperature));
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Displays DSCOVR magnetometer data in table format
    /// </summary>
    public static void DisplayDscovrMagnetometer(IReadOnlyList<DscovrMagnetometerRecord> data, int limit)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]DSCOVR Magnetometer Data[/] [dim](showing {Math.Min(limit, data.Count)} of {data.Count})[/]")
            .AddColumn("[cyan]DateTime[/]")
            .AddColumn("[cyan]Bx (nT)[/]")
            .AddColumn("[cyan]By (nT)[/]")
            .AddColumn("[cyan]Bz (nT)[/]")
            .AddColumn("[cyan]Bt (nT)[/]")
            .AddColumn("[cyan]Lat[/]")
            .AddColumn("[cyan]Lon[/]");

        foreach (var record in data.TakeLast(limit))
        {
            table.AddRow(
                record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Format(record.Bx),
                Format(record.By),
                Format(record.Bz),
                Format(record.Bt),
                Format(record.Latitude),
                Format(record.Longitude));
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Displays DSCOVR solar wind plasma data in table format
    /// </summary>
    public static void DisplayDscovrPlasma(IReadOnlyList<DscovrSolarWindPlasmaRecord> data, int limit)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]DSCOVR Solar Wind Plasma Data[/] [dim](showing {Math.Min(limit, data.Count)} of {data.Count})[/]")
            .AddColumn("[cyan]DateTime[/]")
            .AddColumn("[cyan]Proton Density[/]")
            .AddColumn("[cyan]Bulk Speed[/]")
            .AddColumn("[cyan]Ion Temp[/]");

        foreach (var record in data.TakeLast(limit))
        {
            table.AddRow(
                record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Format(record.ProtonDensity),
                Format(record.BulkSpeed),
                Format(record.IonTemperature));
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Displays KP index forecast data in table format
    /// </summary>
    public static void DisplayKpForecast<T>(IReadOnlyList<T> data, int limit, Func<T, (string date, string kp, string level)> formatter)
    {
        var title = typeof(T).Name.Contains("27Day") ? "27-Day Forecast" : "3-Day Forecast";
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]KP Index {title}[/] [dim](showing {Math.Min(limit, data.Count)} of {data.Count})[/]")
            .AddColumn("[cyan]Date/Time[/]")
            .AddColumn("[cyan]KP Index[/]")
            .AddColumn("[cyan]Activity Level[/]");

        var records = typeof(T).Name.Contains("27Day") ? data.Take(limit) : data.TakeLast(limit);
        foreach (var record in records)
        {
            var (date, kp, level) = formatter(record);
            table.AddRow(date, kp, GetColoredLevel(level));
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Displays KP index nowcast data in table format
    /// </summary>
    public static void DisplayKpNowcast(IReadOnlyList<KpIndexNowcastResponse> data, int limit)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]KP Index Nowcast[/] [dim](showing {Math.Min(limit, data.Count)} of {data.Count})[/]")
            .AddColumn("[cyan]DateTime[/]")
            .AddColumn("[cyan]KP Index[/]")
            .AddColumn("[cyan]Stations[/]")
            .AddColumn("[cyan]Activity Level[/]");

        foreach (var record in data.TakeLast(limit))
        {
            var level = GetActivityLevel((int)record.KpIndex);
            table.AddRow(
                record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                record.KpIndex.ToString("F2"),
                record.StationsCount.ToString(),
                GetColoredLevel(level));
        }

        AnsiConsole.Write(table);
    }

    // Helper methods
    private static string Format(float? value) => value?.ToString("F2") ?? "[dim]N/A[/]";

    private static string GetActivityLevel(int kpIndex) => kpIndex switch
    {
        <= 2 => "Low",
        <= 4 => "Moderate",
        <= 6 => "Elevated",
        <= 8 => "High",
        _ => "Extreme"
    };

    private static string GetColoredLevel(string level) => level switch
    {
        "Low" => "[green]Low[/]",
        "Moderate" => "[yellow]Moderate[/]",
        "Elevated" => "[orange1]Elevated[/]",
        "High" => "[red]High[/]",
        "Extreme" => "[bold red]Extreme[/]",
        _ => level
    };
}
