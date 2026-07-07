namespace AuroraScienceHub.Integrations.NoaaClient;

/// <summary>
/// NOAA client options
/// </summary>
public sealed class NoaaClientOptions
{
    /// <summary>
    /// Configuration option key
    /// </summary>
    public const string OptionKey = "Noaa";

    private const string EmptyServerUrlMessage = $"Configuration value '{OptionKey}:{nameof(ServerUrl)}' is not set.";

    /// <summary>
    /// NOAA server URL
    /// </summary>
    public Uri? ServerUrl { get; set; }

    /// <summary>
    /// Gets the required server URL. Throws <see cref="ArgumentNullException"/> if not set.
    /// </summary>
    public Uri RequiredServerUrl => ServerUrl ?? throw new ArgumentNullException(nameof(ServerUrl), EmptyServerUrlMessage);
}
