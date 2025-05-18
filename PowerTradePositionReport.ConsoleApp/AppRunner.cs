using Microsoft.Extensions.Logging;
using PositionReport.Application.PowerPositionScheduler;

namespace PositionReport.ConsoleApp
{
    public class AppRunner
    {
        private readonly ILogger<AppRunner> _logger;
        private readonly IPowerPositionScheduler _scheduler;
        public AppRunner(ILogger<AppRunner> logger, IPowerPositionScheduler scheduler)
        {
            _logger = logger;
            _scheduler = scheduler;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Application running...");

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                _logger.LogInformation("Cancellation requested. Exiting...");
                e.Cancel = true; // Prevent the process from terminating.
                cts.Cancel();
            };

            await _scheduler.RunAsync(cts.Token);
        }
    }
}
