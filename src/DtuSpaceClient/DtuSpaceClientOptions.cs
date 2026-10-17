namespace AuroraScienceHub.Integrations.DtuSpaceClient;

/// <summary>
/// DTU Space client options
/// </summary>
public sealed class DtuSpaceClientOptions
{
    /// <summary>
    /// Configuration option key
    /// </summary>
    public const string OptionKey = "DtuSpace";

    private const string EmptyServerUrlMessage = $"Configuration value '{OptionKey}:{nameof(ServerUrl)}' is not set.";

    /// <summary>
    /// DTU Space server URL
    /// </summary>
    public Uri? ServerUrl { get; set; }

    /// <summary>
    /// Use a proxy
    /// </summary>
    public bool UseProxy { get; set; } = false;

    /// <summary>
    /// Gets the required server URL. Throws <see cref="ArgumentNullException"/> if not set.
    /// </summary>
    public Uri RequiredServerUrl => ServerUrl ?? throw new ArgumentNullException(nameof(ServerUrl), EmptyServerUrlMessage);
}
