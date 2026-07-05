using AuroraScienceHub.Integrations.NoaaClient.Ace.Responses;

namespace AuroraScienceHub.Integrations.NoaaClient.Ace;

/// <summary>
/// ACE Spacecraft client
/// </summary>
[Obsolete(AceObsoleteMessages.ClientAndModels)]
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
