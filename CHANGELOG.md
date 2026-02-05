# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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

[1.0.0]: https://github.com/Aurora-Science-Hub/Integrations/releases/tag/1.0.0

