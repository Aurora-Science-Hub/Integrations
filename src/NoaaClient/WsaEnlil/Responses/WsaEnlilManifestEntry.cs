using System.Text.Json.Serialization;

namespace AuroraScienceHub.Integrations.NoaaClient.WsaEnlil.Responses;

/// <summary>
/// A single frame entry in the WSA-ENLIL animation manifest.
/// </summary>
/// <param name="Url">Relative frame URL, e.g. "/images/animations/enlil/enlil_com2_58426_20250118T120000.jpg".</param>
public sealed record WsaEnlilManifestEntry(
    [property: JsonPropertyName("url")] string Url
);
