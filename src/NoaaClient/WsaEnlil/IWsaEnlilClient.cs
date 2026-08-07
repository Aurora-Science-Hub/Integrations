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

    /// <summary>
    /// Returns the timestamp of the last frame in the WSA-ENLIL animation, without downloading frames or encoding video.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The timestamp extracted from the last frame's URL, or <see langword="null"/> if the manifest is empty or unavailable.
    /// </returns>
    /// <remarks>
    /// Fetches only the manifest JSON (~1 KB). Use this to check whether the animation has been updated
    /// before calling <see cref="GetEnlilAnimationAsync"/>.
    /// </remarks>
    Task<DateTime?> GetLastFrameTimeAsync(CancellationToken cancellationToken = default);
}
