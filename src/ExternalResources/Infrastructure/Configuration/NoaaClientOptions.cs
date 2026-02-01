namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Infrastructure.Configuration;

public sealed class NoaaClientOptions
{
    public const string OptionKey = "Noaa";

    private const string EmptyServerUrlMessage = $"Configuration value '{OptionKey}:{nameof(ServerUrl)}' is not set.";

    public Uri? ServerUrl { get; set; }

    public Uri RequiredServerUrl => ServerUrl ?? throw new ArgumentNullException(nameof(ServerUrl), EmptyServerUrlMessage);
}
