namespace AuroraScienceHub.Integrations.WdcClient;

/// <summary>
/// Configuration for World Data Center for Geomagnetism, Kyoto clients
/// </summary>
public sealed class WdcClientOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string OptionKey = "Wdc";

    /// <summary>
    /// WDC server URL
    /// </summary>
    public Uri? ServerUrl { get; set; }

    /// <summary>
    /// Whether requests use the proxy configured in the Proxy section
    /// </summary>
    public bool UseProxy { get; set; }

    /// <summary>
    /// Required server URL
    /// </summary>
    /// <exception cref="ArgumentNullException">ServerUrl is not configured</exception>
    public Uri RequiredServerUrl => ServerUrl ?? throw new ArgumentNullException(
        nameof(ServerUrl), $"Configuration value '{OptionKey}:{nameof(ServerUrl)}' is not set.");
}
