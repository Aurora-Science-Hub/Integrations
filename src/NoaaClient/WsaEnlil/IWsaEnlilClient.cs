namespace AuroraScienceHub.Integrations.NoaaClient.WsaEnlil;

/// <summary>
/// Client for creating WSA-ENLIL solar wind forecast animations from NOAA SWPC imagery.
/// </summary>
public interface IWsaEnlilClient
{
    /// <summary>
    /// Downloads the WSA-ENLIL animation manifest and all frames, assembling them into an optimized animated WebP.
    /// </summary>
    /// <param name="maxWidth">Maximum output width in pixels. Frames are resized proportionally. Default 480.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream containing the WebP animation data. The caller is responsible for disposing this stream.</returns>
    /// <remarks>
    /// The returned stream is a MemoryStream containing the complete WebP data.
    /// Typical usage: <c>await using var stream = await GetEnlilAnimationAsync(cancellationToken);</c>
    /// </remarks>
    Task<Stream> GetEnlilAnimationAsync(
        int maxWidth = 480,
        CancellationToken cancellationToken = default);
}
