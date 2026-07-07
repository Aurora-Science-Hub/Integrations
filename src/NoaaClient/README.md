# AuroraScienceHub.Integrations.NoaaClient

NOAA space weather data integration for RTSW and KP-index feeds.

## Overview

Provides HTTP clients for NOAA Space Weather Prediction Center data: real-time solar wind (RTSW) magnetometer and plasma measurements, plus geomagnetic Kp-index nowcast and forecasts.

## Installation

```bash
dotnet add package AuroraScienceHub.Integrations.NoaaClient
```

## Configuration

```json
{
  "Noaa": {
    "ServerUrl": "https://services.swpc.noaa.gov"
  }
}
```

Register clients in DI:

```csharp
builder.Services.AddNoaaClients();
```

## RTSW Client

Primary client for 1-minute magnetometer and solar wind plasma data:

```csharp
var magnetometerData = await rtswClient.GetMagnetometerDataAsync(cancellationToken);
var solarWindData = await rtswClient.GetSolarWindPlasmaDataAsync(cancellationToken);
```

Response models map the NOAA `rtsw_mag_1m.json` and `rtsw_wind_1m.json` schema, including `Active`, `Source`, GSE/GSM field components, and quality flags.

Filter by `Active == true` for the NOAA-selected primary feed, or by `Source` when a specific instrument row is required.

## KP-Index Client

```csharp
var nowcast = await kpIndexClient.GetKpIndexNowcastAsync(cancellationToken);
var forecast3Day = await kpIndexClient.GetKpIndex3DayForecastAsync(cancellationToken);
var forecast27Day = await kpIndexClient.GetKpIndex27DayForecastAsync(cancellationToken);
```

## Deprecated API

`IAceClient` is obsolete and scheduled for removal ([#3](https://github.com/Aurora-Science-Hub/Integrations/issues/3)). Use `IRtswClient` for magnetometer and solar wind plasma data.

## Links

- Repository: https://github.com/Aurora-Science-Hub/Integrations
- Changelog: https://github.com/Aurora-Science-Hub/Integrations/blob/main/CHANGELOG.md
