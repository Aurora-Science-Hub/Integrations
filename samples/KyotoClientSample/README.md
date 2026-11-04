# WDC Kyoto client sample

Interactive Spectre.Console application for real-time hourly Dst data.

```bash
cd samples/KyotoClientSample
dotnet run
```

The menu supports today, yesterday, the last 24 hours and an inclusive UTC day range.
Select **Execute All Requests** to run the preset requests.
Edit `appsettings.json` to configure the Kyoto server and optional proxy.
Missing values appear as `N/A`. Future hours in the day file can also appear as missing.

See the [package README](../../src/KyotoClient/README.md) for time semantics and Kyoto data usage rules.
