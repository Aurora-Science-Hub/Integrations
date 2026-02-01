using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.KpIndex.Responses;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.KpIndex;

/// <summary>
/// KP-index client
/// </summary>
public interface IKpIndexClient
{
    /// <summary>
    /// Get 27-day KP-index forecast
    /// </summary>
    Task<IReadOnlyList<KpIndex27DayResponse>> GetKpIndex27DayForecastAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get 3-day KP-index forecast
    /// </summary>
    Task<IReadOnlyList<KpIndex3DayResponse>> GetKpIndex3DayForecastAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get KP-index nowcast
    /// </summary>
    Task<IReadOnlyList<KpIndexNowcastResponse>> GetKpIndexNowcastAsync(CancellationToken cancellationToken);
}
