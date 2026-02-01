using AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Ace.Responses;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Ace;

/// <summary>
/// ACE Spacecraft client
/// </summary>
public interface IAceClient
{
    /// <summary>
    /// Get ACE Magnetometer data
    /// </summary>
    Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerDataAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get ACE Solar Wind Electron Proton Alpha Monitor data
    /// </summary>
    Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSwepamDataAsync(CancellationToken cancellationToken);
}
