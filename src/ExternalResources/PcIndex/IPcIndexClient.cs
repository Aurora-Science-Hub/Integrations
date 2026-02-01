using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex.Responses;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.PcIndex;

/// <summary>
/// Pc-index client
/// </summary>
public interface IPcIndexClient
{
    /// <summary>
    /// Get Northern Pc-index from AARI web-server
    /// </summary>
    Task<IReadOnlyList<PcIndexResponse>> GetPcNIndexAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get Southern Pc-index from AARI web-server
    /// </summary>
    Task<IReadOnlyList<PcIndexResponse>> GetPcSIndexAsync(CancellationToken cancellationToken);
}
