# Release Notes - Version 1.0.0

**Release Date:** February 5, 2026

## Initial Release

We're excited to announce the first stable release of **Aurora Science Hub Integrations** - a comprehensive suite of .NET clients for accessing space weather data from NOAA data sources.

## Available Packages

### AuroraScienceHub.Integrations.NoaaClient v1.0.0

The flagship package providing seamless integration with NOAA Space Weather Prediction Center data sources.

## Key Features

### ACE Spacecraft Integration
Access real-time data from the Advanced Composition Explorer satellite positioned at the L1 Lagrange point:
- **Magnetometer Data** - Interplanetary magnetic field measurements (Bx, By, Bz components)
- **Solar Wind Plasma Data** - Comprehensive particle measurements including density, speed, and temperature

### RTSW Integration
Retrieve data from NOAA real-time solar wind feeds:
- **1-minute Magnetometer Data** - Real-time interplanetary magnetic field data
- **1-minute Plasma Data** - Real-time bulk solar wind parameters
- **Source Metadata** - Includes `active` and `source` fields in contracts

### KP-Index Data Access
Monitor and forecast geomagnetic activity with three specialized endpoints:
- **Nowcast** - Real-time geomagnetic activity observations
- **3-Day Forecast** - Short-term predictions with 3-hour resolution
- **27-Day Forecast** - Extended outlook for planning and analysis

## Technical Highlights

- **Breaking Change Notice** - `IDscovrClient` and DSCOVR response models are removed in favor of RTSW-only client surface
- **Built on .NET 10.0** - Leveraging the latest .NET features and performance improvements
- **Strongly-Typed APIs** - Compile-time safety with comprehensive response models
- **Dependency Injection** - Native support for ASP.NET Core and modern .NET applications
- **High Performance** - Optimized parsing algorithms validated through extensive benchmarking
- **Production Ready** - Comprehensive unit tests and real-world validation

## Getting Started

Install via NuGet:
```bash
dotnet add package AuroraScienceHub.Integrations.NoaaClient
```

Register services in your application:
```csharp
builder.Services.AddNoaaClients();
```

Start accessing space weather data:
```csharp
// Inject clients via dependency injection
public class SpaceWeatherService
{
    private readonly IAceClient _aceClient;
    private readonly IRtswClient _rtswClient;
    private readonly IKpIndexClient _kpIndexClient;

    public SpaceWeatherService(
        IAceClient aceClient,
        IRtswClient rtswClient,
        IKpIndexClient kpIndexClient)
    {
        _aceClient = aceClient;
        _rtswClient = rtswClient;
        _kpIndexClient = kpIndexClient;
    }

    public async Task<MagnetometerData> GetLatestDataAsync()
    {
        return await _aceClient.GetMagnetometerDataAsync();
    }
}
```

## Documentation

- [Main README](README.md) - Repository overview and development guidelines
- [NoaaClient Documentation](src/NoaaClient/README.md) - Package-specific documentation
- [Sample Application](samples/NoaaClientSample/) - Complete working examples
- [CHANGELOG](CHANGELOG.md) - Detailed change history

## Resources

- **GitHub Repository:** https://github.com/Aurora-Science-Hub/Integrations
- **NuGet Package:** https://www.nuget.org/packages/AuroraScienceHub.Integrations.NoaaClient
- **Issue Tracker:** https://github.com/Aurora-Science-Hub/Integrations/issues
- **Discussions:** https://github.com/Aurora-Science-Hub/Integrations/discussions

## Acknowledgments

This project integrates data from the NOAA Space Weather Prediction Center. We are grateful to NOAA and NASA for providing free and open access to space weather data.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Questions or feedback?** Open an issue on [GitHub](https://github.com/Aurora-Science-Hub/Integrations/issues) or start a [discussion](https://github.com/Aurora-Science-Hub/Integrations/discussions).

