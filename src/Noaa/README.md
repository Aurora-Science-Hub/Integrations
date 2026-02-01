# AuroraScienceHub.Integrations.Noaa

NOAA space weather data integration with support for ACE, DSCOVR spacecraft and KP-index data.

## Overview

Provides unified interfaces for accessing NOAA space weather data including solar wind measurements, magnetometer data, and geomagnetic activity indices.

## Key Features

- **ACE Spacecraft** - Access to Magnetometer and SWEPAM (Solar Wind) data
- **DSCOVR Spacecraft** - Magnetometer and Solar Wind Plasma data with multiple time ranges
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

### DSCOVR Client

```csharp
// Available time ranges: 2H, 1D, 3D, 7D
var magnetometerData = await dscovrClient.GetMagnetometerData1DAsync(cancellationToken);
var solarWindData = await dscovrClient.GetSolarWindPlasmaData1DAsync(cancellationToken);
```

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
