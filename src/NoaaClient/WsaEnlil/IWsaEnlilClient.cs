namespace AuroraScienceHub.Integrations.NoaaClient.WsaEnlil;

/// <summary>
/// Client for creating WSA-ENLIL solar wind forecast animations from NOAA SWPC imagery.
/// </summary>
/// <remarks>
/// Requires FFmpeg to be installed on the system and available in PATH.
/// See README for platform-specific installation instructions.
/// </remarks>
public interface IWsaEnlilClient
{
    /// <summary>
    /// Downloads the WSA-ENLIL animation manifest and all frames, assembling them into an optimized MP4 (H.264) video.
    /// </summary>
    /// <param name="maxWidth">Maximum output width in pixels. Frames are resized proportionally. Default 480.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream containing the MP4 video data. The caller is responsible for disposing this stream.</returns>
    /// <remarks>
    /// The returned stream is a MemoryStream containing the complete MP4 data.
    /// Typical usage: <c>await using var stream = await GetEnlilAnimationAsync(cancellationToken: cancellationToken);</c>
    /// </remarks>
    Task<Stream> GetEnlilAnimationAsync(
        int maxWidth = 480,
        CancellationToken cancellationToken = default);
}
