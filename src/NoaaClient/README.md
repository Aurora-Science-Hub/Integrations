# AuroraScienceHub.Integrations.Noaa

NOAA space weather data integration with support for ACE, RTSW and KP-index data.

## Overview

Provides unified interfaces for accessing NOAA space weather data including solar wind measurements, magnetometer data, and geomagnetic activity indices.

## Key Features

- **ACE Spacecraft** - Access to Magnetometer and SWEPAM (Solar Wind) data
- **RTSW Feed** - 1-minute real-time magnetometer and solar wind plasma data
- **KP-Index** - Geomagnetic activity forecasts and nowcast data
- **Unified Interfaces** - Consistent API across all NOAA data sources

## Installation

```bash
dotnet add package AuroraScienceHub.Integrations.Noaa
```

## Usage

### Service Registration

```csharp
// Configuration
builder.Services.AddNoaaClients();
```

### ACE Client

```csharp
var magnetometerData = await aceClient.GetMagnetometerDataAsync(cancellationToken);
var solarWindData = await aceClient.GetSwepamDataAsync(cancellationToken);
```

### RTSW Client

```csharp
var magnetometerData = await rtswClient.GetMagnetometerDataAsync(cancellationToken);
var solarWindData = await rtswClient.GetSolarWindPlasmaDataAsync(cancellationToken);
```

RTSW response models map the full NOAA `rtsw_mag_1m.json` and `rtsw_wind_1m.json` schema (not legacy DSCOVR field names).

| Magnetometer (`MagnetometerRecord`) | Wind (`SolarWindPlasmaRecord`) |
| --- | --- |
| `DateTime`, `Active`, `Source` | `DateTime`, `Active`, `Source` |
| `Range`, `Scale`, `Sensitivity`, `ManualMode`, `SampleSize` | `ProtonSpeed`, `ProtonTemperature`, `ProtonDensity` |
| `Bt`, `BxGse`–`PhiGse`, `BxGsm`–`PhiGsm` | `ProtonVxGse`–`ProtonVzGsm`, `ProtonSampleSize` |
| `MaxTelemetryFlag`, `MaxDataFlag`, `OverallQuality` | `AlphaSpeed`–`AlphaSampleSize`, quality flags (`MaxConvergenceFlag`–`OverallQuality`) |

### KP-Index Client

```csharp
var nowcast = await kpIndexClient.GetKpIndexNowcastAsync(cancellationToken);
var forecast3Day = await kpIndexClient.GetKpIndex3DayForecastAsync(cancellationToken);
var forecast27Day = await kpIndexClient.GetKpIndex27DayForecastAsync(cancellationToken);
```

### Configuration

```json
{
  "Noaa": {
    "ServerUrl": "https://services.swpc.noaa.gov"
  }
}
```


## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Http` - HTTP utilities
- `AuroraScienceHub.Framework.Utilities` - Common utilities
