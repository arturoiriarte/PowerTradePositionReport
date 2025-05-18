using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PositionReport.Application;
using PositionReport.Application.AmbiguousTimeStrategy;
using PositionReport.Application.Configuration;
using PositionReport.Application.Interfaces;
using PositionReport.Application.PowerPositionRunner;
using PositionReport.Application.PowerPositionScheduler;
using PositionReport.Application.PowerTradeAggregator;
using PositionReport.Application.TimeZoneProvider;
using PositionReport.Infrastructure;

namespace PositionReport.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            var app = host.Services.GetRequiredService<AppRunner>();
            await app.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    // Configuration sources
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    builder.AddCommandLine(args); // optional override settings via command line
                    builder.AddEnvironmentVariables(); // optional override settings via environment variables
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Configuration
                    services
                        .Configure<SchedulerSettings>(hostContext.Configuration.GetSection("SchedulerSettings"))
                        .Configure<ReportExportSettings>(hostContext.Configuration.GetSection("ReportSettings"))
                        .Configure<TimeZoneSettings>(hostContext.Configuration.GetSection("TimezoneSettings"));

                    // DI container
                    services
                        .AddSingleton<AppRunner>()
                        .AddApplication()
                        .AddInfrastructure();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
    }
}