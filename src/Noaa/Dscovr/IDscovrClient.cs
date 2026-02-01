using AuroraScienceHub.Integrations.Noaa.Dscovr.Responses;

namespace AuroraScienceHub.Integrations.Noaa.Dscovr;

/// <summary>
/// DSCOVR Spacecraft client
/// </summary>
public interface IDscovrClient
{
    /// <summary>
    /// Get DSCOVR Magnetometer data (2 hours)
    /// </summary>
    Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData2HAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get DSCOVR Magnetometer data (1 day)
    /// </summary>
    Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData1DAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get DSCOVR Magnetometer data (3 days)
    /// </summary>
    Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData3DAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get DSCOVR Magnetometer data (7 days)
    /// </summary>
    Task<IReadOnlyList<MagnetometerRecord>> GetMagnetometerData7DAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get DSCOVR Solar Wind Electron Proton Alpha Monitor data (2 hours)
    /// </summary>
    Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData2HAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get DSCOVR Solar Wind Electron Proton Alpha Monitor data (1 day)
    /// </summary>
    Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData1DAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get DSCOVR Solar Wind Electron Proton Alpha Monitor data (3 days)
    /// </summary>
    Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData3DAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get DSCOVR Solar Wind Electron Proton Alpha Monitor data (7 days)
    /// </summary>
    Task<IReadOnlyList<SolarWindPlasmaRecord>> GetSolarWindPlasmaData7DAsync(CancellationToken cancellationToken);
}
