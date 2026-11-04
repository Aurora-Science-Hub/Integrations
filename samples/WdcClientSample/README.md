# World Data Center for Geomagnetism, Kyoto client sample

Interactive Spectre.Console application for real-time hourly Dst data.

```bash
cd samples/WdcClientSample
dotnet run
```

The menu supports today, yesterday, the last 24 hours and an inclusive UTC day range.
Select **Execute All Requests** to run the preset requests.
Edit `appsettings.json` to configure the WDC server and optional proxy.
Future hours are excluded from every display and summary. Missing measurements in elapsed hours appear as `N/A`.
The date-range prompt accepts only the previous and current UTC months, through today.
Older dates require WDC provisional/final data, which this real-time sample does not request.
HTTP failures, including `403`, remain errors. The sample disables host logging and renders tables after requests complete.

See the [package README](../../src/WdcClient/README.md) for time semantics and WDC data usage rules.
