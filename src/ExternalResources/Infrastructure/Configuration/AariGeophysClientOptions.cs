namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Infrastructure.Configuration;

/// <summary>
/// HTTP client options for geophysics data import.
/// </summary>
public sealed class AariGeophysClientOptions
{
    public const string OptionKey = "AariGeophys";

    public Uri? ServerUrl { get; init; }

    public Uri RequiredServerUrl => EnsureNotNull(ServerUrl, nameof(ServerUrl));

    private static T EnsureNotNull<T>(T? value, string name)
        where T : class
        => value ?? throw new ArgumentNullException(name, $"Configuration value '{OptionKey}:{name}' is not set.");
}
