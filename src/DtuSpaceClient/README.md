# AuroraScienceHub.Integrations.DtuSpaceClient

DTU Space data integration for the Polar Cap North (PCN) index.

## Overview

Provides HTTP clients for [DTU Space](https://www.space.dtu.dk/) (National Space Institute, Technical University of Denmark) data published at the World Data Center server `ftp.space.dtu.dk/WDC/`. No authorization is required.

## Installation

```bash
dotnet add package AuroraScienceHub.Integrations.DtuSpaceClient
```

## Configuration

```json
{
  "DtuSpace": {
    "ServerUrl": "https://ftp.space.dtu.dk",
    "UseProxy": false
  },
  "Proxy": {
    "Address": "http://proxy.example.com:8080",
    "UserName": "",
    "Password": ""
  }
}
```

- **DtuSpace:ServerUrl** — DTU Space server base URL (required).
- **DtuSpace:UseProxy** — when `true`, client requests are routed through the proxy configured in the `Proxy` section. Default: `false`.
- **Proxy:Address** — proxy server URI (required when `UseProxy` is enabled; startup fails with `InvalidOperationException` if missing).
- **Proxy:UserName** / **Proxy:Password** — optional proxy credentials. When omitted, default credentials are used.

Register clients in DI:

```csharp
builder.Services.AddDtuSpaceClients(builder.Configuration);
```

## PC-Index Client

1-minute Polar Cap North (PCN) index (Thule station, unit mV/m) for a UTC day:

```csharp
var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
var records = await pcIndexClient.GetPcnAsync(today, cancellationToken);

// Range of UTC days, both ends inclusive (e.g. the last hours across midnight UTC)
var lastDays = await pcIndexClient.GetPcnAsync(today.AddDays(-1), today, cancellationToken);
```

- Data is the near-real-time **quick-look** PCN: preliminary values, published with a lag of about 10 minutes. The current day file is appended during the day.
- Quick-look files are retained by DTU Space for roughly the last 31 days.
- Missing values are returned as `null`.
- An empty list is returned when the day file is not published (shortly after 00:00 UTC, for future dates, or for dates outside the quick-look archive).
- The range overload requests day files sequentially (one request per day), skips unpublished days and returns records in time order; `from` later than `to` throws `ArgumentOutOfRangeException`.

Data license: [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/).
