using System.Text.Json.Serialization;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;
using AuroraScienceHub.Integrations.NoaaClient.WsaEnlil.Responses;

namespace AuroraScienceHub.Integrations.NoaaClient.Json;

/// <summary>
/// Source-generated JSON serialization metadata for NOAA response models.
/// Required for trimming and NativeAOT compatibility.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<MagnetometerRecord>))]
[JsonSerializable(typeof(List<SolarWindPlasmaRecord>))]
[JsonSerializable(typeof(List<WsaEnlilManifestEntry>))]
internal sealed partial class NoaaJsonSerializerContext : JsonSerializerContext
{
}
