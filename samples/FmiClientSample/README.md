# Finnish Meteorological Institute client sample

Interactive Spectre.Console application for FMI EURISGIC real-time IU and IL electrojet indices.

```bash
cd samples/FmiClientSample
dotnet run
```

Select **EURISGIC Latest 24 Hours** to retrieve both indices with one request.
Edit `appsettings.json` to configure the FMI server and optional proxy.
The latest records are displayed with their UTC timestamps and magnetometer station counts.

See the [package README](../../src/FmiClient/README.md) for data semantics and usage rules.
