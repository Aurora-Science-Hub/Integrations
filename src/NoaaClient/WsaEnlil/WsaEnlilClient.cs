using AuroraScienceHub.Framework.Http;
using AuroraScienceHub.Integrations.NoaaClient.WsaEnlil.Responses;
using ImageMagick;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Integrations.NoaaClient.WsaEnlil;

internal sealed class WsaEnlilClient : IWsaEnlilClient
{
    private const string ManifestPath = "products/animations/enlil.json";
    private const int FrameDelayMs = 50;
    private const int DefaultColorCount = 256;

    private readonly HttpClient _httpClient;
    private readonly Uri _baseUrl;

    public WsaEnlilClient(
        HttpClient httpClient,
        IOptions<NoaaClientOptions> options)
    {
        _httpClient = httpClient;
        _baseUrl = options.Value.RequiredServerUrl;
    }

    public async Task<byte[]> GetEnlilAnimationAsync(
        int maxWidth = 480,
        CancellationToken cancellationToken = default)
    {
        var manifestUrl = new Uri(_baseUrl, ManifestPath);
        var manifest = await _httpClient
            .GetFromJsonOrDefaultAsync<List<WsaEnlilManifestEntry>>(manifestUrl, cancellationToken)
            .ConfigureAwait(false);

        if (manifest is null || manifest.Count == 0)
        {
            return [];
        }

        using var collection = new MagickImageCollection();

        foreach (var entry in manifest)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var frameUrl = new Uri(_baseUrl, entry.Url).ToString();
            await using var stream = await _httpClient
                .GetStreamAsync(frameUrl, cancellationToken)
                .ConfigureAwait(false);

            var image = new MagickImage(stream);

            if (image.Width > maxWidth)
            {
                var geometry = new MagickGeometry((uint)maxWidth, 0)
                {
                    IgnoreAspectRatio = false
                };
                image.Resize(geometry);
                image.Strip(); // Remove metadata to reduce file size
            }

            // Magick.NET AnimationDelay is in centiseconds
            image.AnimationDelay = (uint)(FrameDelayMs / 10);
            image.GifDisposeMethod = GifDisposeMethod.Background;

            collection.Add(image);
        }

        collection.Quantize(new QuantizeSettings { Colors = DefaultColorCount });

        return collection.ToByteArray(MagickFormat.Gif);
    }
}
