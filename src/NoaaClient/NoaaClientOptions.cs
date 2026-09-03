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
    /// Use a proxy
    /// </summary>
    public bool UseProxy { get; set; } = false;

    /// <summary>
    /// Number of download attempts for a single RTSW feed before giving up.
    /// NOAA occasionally serves a truncated/partial JSON body (connection cut near
    /// the end of the file); retrying the whole download usually returns a full body.
    /// Defaults to 3 attempts.
    /// </summary>
    public int RtswRetryCount { get; set; } = 3;

    /// <summary>
    /// Delay between RTSW download retries.
    /// Defaults to 2 seconds.
    /// </summary>
    public TimeSpan RtswRetryDelay { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets the required server URL. Throws <see cref="ArgumentNullException"/> if not set.
    /// </summary>
    public Uri RequiredServerUrl => ServerUrl ?? throw new ArgumentNullException(nameof(ServerUrl), EmptyServerUrlMessage);
}
