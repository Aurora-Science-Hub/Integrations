# NOAA Client Sample

An interactive console application demonstrating the NOAA API clients using Spectre.Console.

## Overview

This sample demonstrates all available NOAA client methods through an interactive menu-driven interface:

### ACE (Advanced Composition Explorer) Spacecraft
- **Magnetometer Data** - 1-minute averaged magnetic field measurements
- **SWEPAM Data** - Solar Wind Electron Proton Alpha Monitor measurements

### RTSW (Real-Time Solar Wind) Feeds
- **Magnetometer Data** - 1-minute real-time magnetic field measurements
- **Solar Wind Plasma Data** - 1-minute real-time solar wind measurements

### KP Index
- **27-Day Forecast** - Extended KP index forecast
- **3-Day Forecast** - Short-term KP index forecast
- **Nowcast** - Real-time KP index measurements

## Features

**Interactive Menu** - Easy-to-use selection interface powered by Spectre.Console

**Formatted Tables** - Beautiful table output with colors and borders

**Activity Levels** - KP index displayed with color-coded activity levels:
- Low (0-2) - Green
- Moderate (3-4) - Yellow
- Elevated (5-6) - Orange
- High (7-8) - Red
- Extreme (9+) - Bold Red

**Bulk Execution** - Execute all API requests with a single menu option

**Loading Indicators** - Visual feedback with spinners and progress bars

## Requirements

- .NET 8.0 or higher
- Internet connection (for NOAA API access)

## Configuration

The application uses `appsettings.json` for configuration:

```json
{
  "Noaa": {
    "ServerUrl": "https://services.swpc.noaa.gov"
  }
}
```

## Usage

Run the application:

```bash
dotnet run
```

You'll see an interactive menu with the following options:

1. **ACE Magnetometer** - Fetch ACE spacecraft magnetometer data
2. **ACE SWEPAM** - Fetch ACE solar wind plasma data
3. **RTSW Magnetometer** - Fetch RTSW magnetometer data
4. **RTSW Solar Wind Plasma** - Fetch RTSW plasma data
5. **KP Index 27-Day Forecast** - View extended KP forecast
6. **KP Index 3-Day Forecast** - View short-term KP forecast
7. **KP Index Nowcast** - View current KP measurements
8. **Execute All Requests** - Run all queries sequentially with progress tracking
9. **Exit** - Quit the application

Simply use arrow keys to navigate and press Enter to select an option.

## Architecture

The application follows best practices:

- **Dependency Injection** - Uses Microsoft.Extensions.DependencyInjection
- **Configuration Management** - Leverages Microsoft.Extensions.Configuration
- **Typed HTTP Clients** - Registered via AddHttpClient
- **Async/Await** - All API calls are asynchronous
- **Error Handling** - Graceful error handling with user-friendly messages
- **Separation of Concerns** - UI formatting separated in OutputFormatter class

## Project Structure

```
NoaaClientSample/
├── Program.cs              # Main application with interactive menu
├── OutputFormatter.cs      # Table formatting utilities
├── appsettings.json        # Application configuration
└── NoaaClientSample.csproj # Project file
```

## Dependencies

- **AuroraScienceHub.Integrations.Noaa** - NOAA API client library
- **Microsoft.Extensions.Hosting** - For DI and configuration
- **Spectre.Console** - For beautiful console UI

## Sample Output

```
  _   _  ___    _    _        ____ _ _            _
 | \ | |/ _ \  / \  / \      / ___| (_) ___ _ __ | |_
 |  \| | | | |/ _ \/ _ \    | |   | | |/ _ \ '_ \| __|
 | |\  | |_| / ___ \ ___ \   | |___| | |  __/ | | | |_
 |_| \_|\___/_/   \_\   \_\  \____|_|_|\___|_| |_|\__|

Interactive sample application for NOAA API clients

? Select an option: ›
❯ ACE Magnetometer
  ACE SWEPAM
  RTSW Magnetometer
  RTSW Solar Wind Plasma
  KP Index 27-Day Forecast
  KP Index 3-Day Forecast
  KP Index Nowcast
  ──────────────────────────────
  Execute All Requests
  Exit
```

## License

See LICENSE file in the repository root.


