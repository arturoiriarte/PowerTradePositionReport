# PowerTrade Position Aggregator

A .NET 9 console application that aggregates day-ahead power positions per hour and outputs them to a CSV file.

## Technologies

- C#, .NET 9
- `xUnit`, `Moq`, `FluentAssertions` for testing

## Configuration

### Update `appsettings.json`:

```json
{
  "SchedulerSettings": {
    "TimeIntervalInMinutes": 1,
    "MaxRetryAttempts": 3
  },
  "ReportExportSettings": {
    "OutputPath": "./reports"
  },
  "TimezoneSettings": {
    "TimeZoneId": "Europe/London"
  }
}
```

### Override with CLI:

```bash
dotnet run --SchedulerSettings:TimeIntervalInMinutes=1 --ReportExportSettings:OutputPath='./reports'
```

### Override with Environment variables

Use `__` to represent nested JSON keys

```bash
SchedulerSettings__MaxRetryAttempts=5
ReportExportSettings__OutputPath=./output
```

Alternative if launching from Visual Studio you can use `launchSettings.json`

```json
{
  "profiles": {
    "PowerTradePositionReport.ConsoleApp": {
      "commandName": "Project",
      "environmentVariables": {
        "SERVICE_MODE": "Normal",
        "SchedulerSettings__MaxRetryAttempts": "5"
      }
    },
    "Container (Dockerfile)": {
      "commandName": "Docker"
    }
  }
}
```

## Usage

```bash
cd PowerTradePositionReport.ConsoleApp
dotnet run
```

## Test

Change directory into

```bash
cd PositionReport.Application.Tests
dotnet test
```

```bash
cd PositionReport.Infrastructure.Tests
dotnet test
```
