using System.Globalization;
using System.Text.RegularExpressions;
using AuroraScienceHub.Framework.Http;
using AuroraScienceHub.Integrations.NoaaClient.WsaEnlil.Responses;
using FFMpegCore;
using FFMpegCore.Pipes;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.NoaaClient.WsaEnlil;

internal sealed partial class WsaEnlilClient : IWsaEnlilClient
{
    private const string ManifestPath = "products/animations/enlil.json";
    private const int Fps = 20;
    private const int Crf = 23; // H.264 quality (0 = lossless, 51 = worst)
    private const string TempDirPrefix = "enlil_";
    private const string FrameFileFormat = "frame_{0:D4}.jpg";
    private const string FrameSearchPattern = "frame_%04d.jpg";
    private const int OutputStreamCapacity = 5 * 1024 * 1024; // 5 MB initial buffer

    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    [GeneratedRegex(@"(\d{8}T\d{6})\.jpg$")]
    private static partial Regex FrameTimestampRegex();

    public WsaEnlilClient(
        HttpClient httpClient,
        IOptions<NoaaClientOptions> options)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<Stream> GetEnlilAnimationAsync(
        int maxWidth,
        CancellationToken cancellationToken)
    {
        if (maxWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWidth), "Max width must be greater than zero.");
        }

        var manifest = await FetchManifestAsync(cancellationToken).ConfigureAwait(false);
        if (manifest is null || manifest.Count == 0)
        {
            return new MemoryStream();
        }

        string? tempDir = null;
        try
        {
            tempDir = Directory.CreateTempSubdirectory(TempDirPrefix).FullName;
            await DownloadFramesAsync(manifest, tempDir, cancellationToken).ConfigureAwait(false);
            return await EncodeVideoAsync(tempDir, maxWidth, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (tempDir is not null)
            {
                try { Directory.Delete(tempDir, recursive: true); }
                catch { /* best-effort cleanup */ }
            }
        }
    }

    public async Task<DateTime?> GetLastFrameTimeAsync(CancellationToken cancellationToken)
    {
        var manifest = await FetchManifestAsync(cancellationToken).ConfigureAwait(false);
        if (manifest is null || manifest.Count == 0)
        {
            return null;
        }

        var lastUrl = manifest.Last().Url;
        var match = FrameTimestampRegex().Match(lastUrl);
        if (!match.Success)
        {
            return null;
        }

        return DateTime.ParseExact(
            match.Groups[1].Value,
            "yyyyMMddTHHmmss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }

    private async Task<IReadOnlyCollection<WsaEnlilManifestEntry>?> FetchManifestAsync(
        CancellationToken cancellationToken)
    {
        var manifestUrl = new Uri(_baseUrl, ManifestPath);
        return await _httpClient
            .GetFromJsonOrDefaultAsync<IReadOnlyCollection<WsaEnlilManifestEntry>>(manifestUrl, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task DownloadFramesAsync(
        IReadOnlyCollection<WsaEnlilManifestEntry> manifest,
        string tempDir,
        CancellationToken cancellationToken)
    {
        var index = 0;
        foreach (var entry in manifest)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var frameUrl = new Uri(_baseUrl, entry.Url);
            await using var sourceStream = await _httpClient
                .GetStreamAsync(frameUrl, cancellationToken)
                .ConfigureAwait(false);

            var framePath = Path.Combine(tempDir, string.Format(FrameFileFormat, index));
            await using var fileStream = File.Create(framePath);
            await sourceStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

            index++;
        }
    }

    private static async Task<MemoryStream> EncodeVideoAsync(
        string tempDir,
        int maxWidth,
        CancellationToken cancellationToken)
    {
        var outputStream = new MemoryStream(OutputStreamCapacity);
        var inputPattern = Path.Combine(tempDir, FrameSearchPattern);

        await FFMpegArguments
            .FromFileInput(inputPattern, verifyExists: false,
                inputOptions => inputOptions
                    .WithCustomArgument($"-framerate {Fps}"))
            .OutputToPipe(new StreamPipeSink(outputStream),
                outputOptions => outputOptions
                    .WithCustomArgument($"-vf scale={maxWidth}:-2")
                    .WithVideoCodec("libx264")
                    .WithCustomArgument($"-crf {Crf}")
                    .WithCustomArgument("-pix_fmt yuv420p")
                    .WithCustomArgument("-movflags +frag_keyframe+empty_moov")
                    .ForceFormat("mp4"))
            .CancellableThrough(cancellationToken)
            .ProcessAsynchronously()
            .ConfigureAwait(false);

        outputStream.Position = 0;
        return outputStream;
    }
}
