<h1 align="center">
    <br>
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="docs/logo/In_white.png">
      <source media="(prefers-color-scheme: light)" srcset="docs/logo/In_black.png">
      <img src="docs/logo/logo-black.png" style="width:400px;">
    </picture>
    <br>
    Aurora Science Hub Integrations
    <br>
</h1>
<div align="center">
    External data source integrations for space weather monitoring applications.
    <br><br>

[![NuGet Version](https://img.shields.io/nuget/v/AuroraScienceHub.Integrations.NoaaClient?logo=nuget&label=NuGet)](https://www.nuget.org/packages/AuroraScienceHub.Integrations.NoaaClient/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AuroraScienceHub.Integrations.NoaaClient?logo=nuget&label=Downloads)](https://www.nuget.org/packages/AuroraScienceHub.Integrations.NoaaClient/)
[![](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
<br>
[![Build & Test](https://github.com/Aurora-Science-Hub/Integrations/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Aurora-Science-Hub/Integrations/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![GitHub Stars](https://img.shields.io/github/stars/Aurora-Science-Hub/Integrations?style=social)](https://github.com/Aurora-Science-Hub/Integrations)

<a href="#getting-started">Getting Started</a> •
<a href="#available-packages">Available Packages</a> •
<a href="#architecture">Architecture</a> •
<a href="#development">Development</a> •
<a href="#code-style">Code Style</a> •
<a href="#testing">Testing</a> •
<a href="#license">License</a> •
<a href="#contributing">Contributing</a> •
<a href="#links">Links</a>

</div>


## Getting Started

All packages are distributed via NuGet and target **.NET 10.0**. Install individual packages as needed:

```bash
dotnet add package AuroraScienceHub.Integrations.NoaaClient
```

### Quick Example

```csharp
// Register services
builder.Services.AddNoaaClients();

// Inject and use clients
public class SpaceWeatherService
{
    private readonly IAceClient _aceClient;

    public SpaceWeatherService(IAceClient aceClient)
    {
        _aceClient = aceClient;
    }

    public async Task<IReadOnlyCollection<MagnetometerRecord>> GetMagnetometerDataAsync()
    {
        return await _aceClient.GetMagnetometerDataAsync();
    }
}
```

Each package provides HTTP clients for accessing external data sources with strongly-typed response models and full dependency injection support.

## Available Packages

### NOAA Space Weather Integrations

**[AuroraScienceHub.Integrations.NoaaClient](src/NoaaClient/)** - Comprehensive NOAA Space Weather data access

| Component           | Description                                                                                                                 |
|---------------------|-----------------------------------------------------------------------------------------------------------------------------|
| **ACE Client**      | Advanced Composition Explorer satellite data - magnetometer and SWEPAM measurements                                         |
| **DSCOVR Client**   | Deep Space Climate Observatory data - solar wind and magnetic field measurements with multiple time ranges (2H, 1D, 3D, 7D) |
| **KP-Index Client** | Geomagnetic activity indices - nowcast and forecast data (3-day and 27-day)                                                 |

See [detailed documentation](src/NoaaClient/README.md) for usage examples and API reference.

## Architecture

This repository provides production-ready HTTP clients for external space weather data sources with the following characteristics:

- **Type-Safe Clients** - Strongly-typed HTTP clients with dependency injection support via `IHttpClientFactory`
- **Response Models** - Well-defined DTOs for all API responses with validation
- **Configuration** - Options pattern for configuring API endpoints and behavior
- **Modern .NET** - Built on .NET 10 with latest C# features and performance optimizations
- **Resilience** - Built-in retry policies and error handling
- **Testability** - Designed for easy unit testing with interface-based design

### Design Principles

- **Single Responsibility** - Each client focuses on a specific data source
- **Dependency Injection** - First-class DI support for ASP.NET Core and .NET applications
- **Performance** - Optimized parsing and minimal allocations
- **Extensibility** - Easy to extend with additional data sources

## Development

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- IDE: [Visual Studio 2025+](https://visualstudio.microsoft.com/), [Rider 2025+](https://www.jetbrains.com/rider/), or [VS Code](https://code.visualstudio.com/)

### Building from Source

```bash
# Clone the repository
git clone https://github.com/Aurora-Science-Hub/Integrations.git
cd Integrations

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

## Code Style

The solution uses [EditorConfig](.editorconfig) based on [Azure SDK .NET](https://github.com/Azure/azure-sdk-for-net/blob/main/.editorconfig) to maintain consistent code style across all packages.

### Formatting Commands

```bash
# Format code before committing
dotnet format

# Verify code style compliance
dotnet format --verify-no-changes
```

### Code Quality Standards

- Nullable reference types enabled
- Warnings treated as errors
- Latest C# language version
- Code style enforcement in build
- Embedded debug symbols in packages

## Testing

Unit tests are located in the `tests/UnitTests/` directory.

### Test Framework

- **xUnit** - Test execution framework
- **Moq** - Mocking library for unit tests
- **Embedded Resources** - Test data stored as embedded resources for reproducibility

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests in watch mode
dotnet watch test --project tests/UnitTests/UnitTests.csproj
```

### Test Coverage

The project maintains comprehensive test coverage for all data parsers and client implementations, ensuring data integrity and API reliability.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) file for details.

## Contributing

We welcome contributions! When contributing to this repository:

1. Follow the established code style (enforced by EditorConfig)
2. Run `dotnet format` before committing
3. Ensure all tests pass with `dotnet test`
4. Update relevant README files for your changes
5. Keep packages focused and loosely coupled
6. Add unit tests for new features

### Reporting Issues

Please report bugs and feature requests on the [GitHub Issues](https://github.com/Aurora-Science-Hub/Integrations/issues) page.

## Links

- **NuGet Package**: [AuroraScienceHub.Integrations.NoaaClient](https://www.nuget.org/packages/AuroraScienceHub.Integrations.NoaaClient/)
- **Source Code**: [GitHub Repository](https://github.com/Aurora-Science-Hub/Integrations)
- **Issue Tracker**: [GitHub Issues](https://github.com/Aurora-Science-Hub/Integrations/issues)
- **Changelog**: [CHANGELOG.md](CHANGELOG.md)
- **Release Notes**: [RELEASE_NOTES.md](docs/ReleaseNotes_Integrations_v1.md)

## Acknowledgments

This project integrates data from the **NOAA Space Weather Prediction Center**. We thank NOAA and NASA for providing free and open access to space weather data.

---

**Note**: This project provides data source integrations for space weather monitoring applications.
