# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.1] - 2026-07-07

### Fixed

#### NoaaClient Package
- Removed trailing dot from `IAceClient` obsolete message URL
- NuGet package README now ships package-local markdown instead of repository HTML landing page

### Changed

#### NoaaClient Package
- Repository and package docs updated to RTSW-first examples (deprecated ACE usage removed from primary docs)
- **ACE client deprecated** — only `IAceClient` marked `[Obsolete]`; removal tracked in [#3](https://github.com/Aurora-Science-Hub/Integrations/issues/3)

## [1.1.0] - 2026-07-07

### Added

#### NoaaClient Package
- **RTSW Client** - NOAA real-time solar wind integration
  - `IRtswClient` with 1-minute magnetometer and plasma feeds
  - JSON deserialization for `rtsw_mag_1m.json` and `rtsw_wind_1m.json`
  - Response records map the full NOAA RTSW schema (`BxGsm`, `ProtonDensity`, quality flags, etc.), not legacy DSCOVR field names

### Removed

#### NoaaClient Package
- **DSCOVR Client Surface** (breaking change)
  - Removed `IDscovrClient`, related implementations, models, tests, and benchmarks
  - Removed DSCOVR sample app menu entries

### Changed

#### NoaaClient Package
- **RTSW response models** (breaking change)
  - Magnetometer and wind records use live NOAA RTSW property names
  - Removed misleading aliases (`Latitude`/`Longitude`, `BulkSpeed`/`IonTemperature`)

#### Dependency Injection
- `AddNoaaClients()` now registers `IRtswClient` instead of `IDscovrClient`

## [1.0.0] - 2026-02-05

### Added

#### NoaaClient Package
- **ACE Client** - Integration with Advanced Composition Explorer (ACE) satellite data
  - Magnetometer data access with real-time measurements
  - Solar Wind Plasma data (SWEPAM) with comprehensive particle measurements
  - Strongly-typed response models for all data types

- **DSCOVR Client** - Deep Space Climate Observatory (DSCOVR) satellite integration
  - Magnetometer data with multiple time ranges (2H, 1D, 3D, 7D)
  - Solar Wind Plasma data with flexible time window selection
  - Unified interface consistent with ACE client

- **KP-Index Client** - Geomagnetic activity index monitoring
  - Real-time nowcast data access
  - 3-day forecast with hourly resolution
  - 27-day forecast for long-term planning
  - Parsed and validated response models

#### Core Features
- HTTP client implementations with dependency injection support
- Configuration through Options pattern
- Strongly-typed response models for all API endpoints
- Built on .NET 10.0 with latest C# features
- Comprehensive unit test coverage
- Performance benchmarks for data parsing operations

#### Documentation
- Complete API documentation with usage examples
- Package-specific README files
- Sample application demonstrating all client capabilities
- XML documentation for all public APIs

#### Development Infrastructure
- EditorConfig for consistent code style
- Automated code formatting with `dotnet format`
- Warnings treated as errors for code quality
- Embedded debug symbols in NuGet packages
- GitHub Actions CI/CD pipeline for build and test

[1.1.1]: https://github.com/Aurora-Science-Hub/Integrations/compare/1.1.0...1.1.1
[1.1.0]: https://github.com/Aurora-Science-Hub/Integrations/compare/1.0.0...1.1.0
[1.0.0]: https://github.com/Aurora-Science-Hub/Integrations/releases/tag/1.0.0
[Unreleased]: https://github.com/Aurora-Science-Hub/Integrations/compare/1.1.1...HEAD

