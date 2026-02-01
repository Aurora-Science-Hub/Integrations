<h1 align="center">
    <br>
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="docs/logo/Fw_white.png">
      <source media="(prefers-color-scheme: light)" srcset="docs/logo/Fw_black.png">
      <img src="docs/logo/logo-black.png" style="width:400px;">
    </picture>
    <br>
    Aurora Science Hub Integrations
    <br>
</h1>
<div align="center">
    External data source integrations for space weather monitoring applications.
    <br><br>

[![](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build & test](https://github.com/Aurora-Science-Hub/Integrations/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Aurora-Science-Hub/Integrations/actions/workflows/dotnet.yml)

<a href="#available-packages">Available Packages</a> •
<a href="#development">Development</a> •
<a href="#code-style">Code Style</a> •
<a href="#testing">Testing</a> •
<a href="#licence">Licence</a> •
<a href="#contributing">Contributing</a>

</div>


## Getting Started

All packages are distributed via NuGet and target **.NET 9.0**. Install individual packages as needed:

```bash
dotnet add package AuroraScienceHub.Integrations.Noaa
# ... other packages
```

Each package provides HTTP clients for accessing external data sources with strongly-typed response models.

## Available Packages

### NOAA Integrations

- **[Noaa](src/Noaa/)** - NOAA Space Weather data source clients
  - **ACE** - Advanced Composition Explorer satellite data
  - **DSCOVR** - Deep Space Climate Observatory satellite data
  - **Kp Index** - Geomagnetic activity index data

## Architecture

This repository provides HTTP clients for external space weather data sources:

- **Type-Safe Clients** - Strongly-typed HTTP clients with dependency injection support
- **Response Models** - Well-defined DTOs for all API responses
- **Configuration** - Options pattern for configuring API endpoints
- **Modern .NET** - Built on .NET 9 with latest C# features

## Development

### Prerequisites

- .NET 9.0 SDK or later
- IDE with C# support (Rider, Visual Studio, VS Code)

### Building the Solution

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Create NuGet packages
dotnet pack
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

Unit tests are located in the `tests/UnitTests/` directory. The framework uses:

- xUnit for test execution
- AutoFixture with AutoMoq for test data generation
- Moq for mocking

## License

See [LICENSE](LICENSE) and [PackageLicenses.txt](PackageLicenses.txt) for details.

## Contributing

When contributing to this repository:

1. Follow the established code style (enforced by EditorConfig)
2. Run `dotnet format` before committing
3. Ensure all tests pass
4. Update relevant README files for your changes
5. Keep packages focused and loosely coupled

---

**Note**: This project provides data source integrations for the SWeather (Space Weather) monitoring application.
