namespace AuroraScienceHub.Integrations.NoaaClient.WsaEnlil;

/// <summary>
/// Client for creating WSA-ENLIL solar wind forecast animated GIFs from NOAA SWPC imagery.
/// </summary>
public interface IWsaEnlilClient
{
    /// <summary>
    /// Downloads the WSA-ENLIL animation manifest and all frames, assembling them into an optimized animated GIF.
    /// </summary>
    /// <param name="maxWidth">Maximum output width in pixels. Frames are resized proportionally. Default 480.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>GIF file bytes.</returns>
    Task<byte[]> GetEnlilAnimationAsync(
        int maxWidth = 480,
        CancellationToken cancellationToken = default);
}
