# Finnish Meteorological Institute EURISGIC client

`AuroraScienceHub.Integrations.FmiClient` reads real-time IU and IL electrojet indices from the Finnish Meteorological Institute (FMI) EURISGIC service.

```bash
dotnet add package AuroraScienceHub.Integrations.FmiClient
```

## Configuration

```json
{
  "Fmi": {
    "ServerUrl": "https://space.fmi.fi/",
    "UseProxy": false
  }
}
```

Set `UseProxy` to `true` to use the framework's `Proxy` section (`Address`, `UserName`, `Password`).

```csharp
using AuroraScienceHub.Integrations.FmiClient;
using AuroraScienceHub.Integrations.FmiClient.Eurisgic;

builder.Services.AddFmiClients(builder.Configuration);

// Inject IEurisgicClient into the consumer.
var records = await client.GetIndicesAsync(cancellationToken);
```

## Data semantics

- One request retrieves both IU and IL from the [real-time EURISGIC file](https://space.fmi.fi/image/realtime/eurisgic/realtime_iu_il.txt), which covers the latest 24 hours.
- Values are preliminary 1-minute electrojet indices in nT. FMI does not manually check their quality.
- `DateTime` is UTC. The provider occasionally publishes second `60`; the client normalizes such timestamps to the start of the next minute.
- `Iu` and `Il` map `NaN` to `null`. `StationCount` contains the number of magnetometer stations used for the record.
- Comment lines and malformed rows are skipped. An empty response body returns no records; HTTP failures, including `404`, and cancellation propagate to the caller.

## Data usage

The package code is MIT-licensed. FMI data are available under the [Creative Commons Attribution 4.0 International license](https://creativecommons.org/licenses/by/4.0/).
Attribute the Finnish Meteorological Institute when using the data.

See [FmiClientSample](../../samples/FmiClientSample/README.md) for an interactive console application.
