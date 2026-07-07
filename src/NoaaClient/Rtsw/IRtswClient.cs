using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;

namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw;

/// <summary>
/// NOAA real-time solar wind (RTSW) client.
/// </summary>
public interface IRtswClient
{
    /// <summary>
    /// Gets 1-minute real-time interplanetary magnetic field values.
    /// </summary>
    Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerDataAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets 1-minute real-time solar wind plasma values.
    /// </summary>
    Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaDataAsync(CancellationToken cancellationToken);
}
