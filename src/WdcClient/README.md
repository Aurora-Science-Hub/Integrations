# World Data Center for Geomagnetism, Kyoto Dst client

`AuroraScienceHub.Integrations.WdcClient` reads the real-time hourly Dst index from World Data Center for Geomagnetism, Kyoto.

```bash
dotnet add package AuroraScienceHub.Integrations.WdcClient
```

## Configuration

```json
{
  "Wdc": {
    "ServerUrl": "https://wdc.kugi.kyoto-u.ac.jp/",
    "UseProxy": false
  }
}
```

Set `UseProxy` to `true` to use the framework's `Proxy` section (`Address`, `UserName`, `Password`).

```csharp
using AuroraScienceHub.Integrations.WdcClient;
using AuroraScienceHub.Integrations.WdcClient.DstIndex;

builder.Services.AddWdcClients(builder.Configuration);

// Inject IDstIndexClient into the consumer.
var day = await client.GetDstAsync(new DateOnly(2026, 11, 1), cancellationToken);
var range = await client.GetDstAsync(
    new DateOnly(2026, 10, 31),
    new DateOnly(2026, 11, 1),
    cancellationToken);
```

## Data semantics

The [WDC format snapshot](References/DstRecordFormat.md) records the provider's layout as retrieved on 2026-11-04.

- Values are preliminary real-time (quick-look) Dst in nT. They can change as WDC revises the raw data. Publication latency varies.
- `DateTime` is UTC and marks the start of the hourly interval. The file's first hourly value maps to `00:00`, the last to `23:00`.
- The [WDC format](https://wdc.kugi.kyoto-u.ac.jp/dstae/format/dstformat.html) contains a base in hundreds of nT, 24 hourly values, and a daily mean. The client adds the base to each present value and ignores the mean.
- `9999` maps to `null`, including hours that WDC has not published yet. Consumers should filter future timestamps as needed.
- Day ranges include both endpoints and may intersect at most two calendar months. Longer ranges throw `ArgumentOutOfRangeException` before sending requests. The client requests each intersecting month once, sequentially, and returns records ordered by time.
- An absent month (`404`) or empty body returns no records. Other HTTP failures and cancellation propagate to the caller.
- The client uses only `dst_realtime/yyyyMM/dstyyMM.for.request`. It does not substitute provisional or final products for an absent real-time month. See the [available archive](https://wdc.kugi.kyoto-u.ac.jp/dst_realtime/index.html).
- Truncated rows, malformed headers, invalid dates and malformed hourly fields are skipped. Valid hours keep their original timestamps.

## Data usage

The package code is MIT-licensed. WDC data have separate [usage rules](https://wdc.kugi.kyoto-u.ac.jp/wdc/Sec3.html).
WDC permits non-commercial use of its geomagnetic indices and requires attribution to World Data Center for Geomagnetism, Kyoto and the data suppliers.
Contact World Data Center for Geomagnetism, Kyoto before using quick-look data in publications or presentations. Include the data DOI when publishing results.

See [WdcClientSample](../../samples/WdcClientSample/README.md) for an interactive console application.
