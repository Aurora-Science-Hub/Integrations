# DTU Space Client Sample

An interactive console application demonstrating the DTU Space API clients using Spectre.Console.

## Overview

This sample demonstrates all available DTU Space client methods through an interactive menu-driven interface:

### PC Index
- **PCN Index Today** - 1-minute quick-look Polar Cap North index for the current UTC day
- **PCN Index Yesterday** - 1-minute quick-look Polar Cap North index for the previous UTC day
- **PCN Index Last 24 Hours** - Previous and current UTC days requested as a range and trimmed to the last 24 hours
- **PCN Index Date Range** - 1-minute quick-look Polar Cap North index for an entered range of UTC days

## Features

**Interactive Menu** - Easy-to-use selection interface powered by Spectre.Console

**Formatted Tables** - Beautiful table output with colors and borders

**Summary Line** - Period, min/max/latest PC index value and the number of missing values

**Bulk Execution** - Execute all API requests with a single menu option

**Loading Indicators** - Visual feedback with spinners and progress bars

## Requirements

- .NET 11.0 or higher
- Internet connection (for DTU Space server access)

## Configuration

The application uses `appsettings.json` for configuration:

```json
{
  "DtuSpace": {
    "ServerUrl": "https://ftp.space.dtu.dk"
  }
}
```

## Usage

Run the application:

```bash
dotnet run
```

You'll see an interactive menu with the following options:

1. **PCN Index Today** - Fetch PCN index for the current UTC day
2. **PCN Index Yesterday** - Fetch PCN index for the previous UTC day
3. **PCN Index Last 24 Hours** - Fetch PCN index for the last 24 hours across midnight UTC
4. **PCN Index Date Range** - Enter `From` and `To` UTC days (`yyyy-MM-dd`, both inclusive) and fetch PCN index for the range
5. **Execute All Requests** - Run all queries sequentially with progress tracking
6. **Exit** - Quit the application

Simply use arrow keys to navigate and press Enter to select an option.

Quick-look data is retained by DTU Space for roughly the last 31 days; days outside the archive or not yet published are reported as having no data.

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
DtuSpaceClientSample/
├── Program.cs                  # Main application with interactive menu
├── OutputFormatter.cs          # Table formatting utilities
├── appsettings.json            # Application configuration
└── DtuSpaceClientSample.csproj # Project file
```

## Dependencies

- **AuroraScienceHub.Integrations.DtuSpaceClient** - DTU Space API client library
- **Microsoft.Extensions.Hosting** - For DI and configuration
- **Spectre.Console** - For beautiful console UI

## License

See LICENSE file in the repository root.
