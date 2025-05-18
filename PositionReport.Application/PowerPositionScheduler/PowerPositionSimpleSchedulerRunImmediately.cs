using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PositionReport.Application.Configuration;
using PositionReport.Application.PowerPositionRunner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.PowerPositionScheduler
{
    public class PowerPositionSimpleSchedulerRunImmediately : IPowerPositionScheduler
    {
        private readonly ILogger<PowerPositionSimpleSchedulerRunImmediately> _logger;
        private readonly ISchedulerSettings _schedulerSettings;
        private readonly IPowerPositionRunnerWithRetry _runner;

        public PowerPositionSimpleSchedulerRunImmediately(
            ILogger<PowerPositionSimpleSchedulerRunImmediately> logger,
            IOptions<SchedulerSettings> schedulerSettings,
            IPowerPositionRunnerWithRetry runner)
        {
            _logger = logger;
            _schedulerSettings = schedulerSettings.Value;
            _runner = runner;
        }

        // Scheduling logic
        // This method would be called to start the scheduler
        // You can use a library like Quartz.NET or Hangfire for scheduling
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler started.");

            // Run on start
            await _runner.RunOnceWithRetryAsync(cancellationToken);

            // Schedule the task to run at a specific interval
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Wait for the next interval
                    await Task.Delay(TimeSpan.FromMinutes(_schedulerSettings.TimeIntervalInMinutes), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Handle cancellation
                    _logger.LogInformation("Scheduler cancelled.");
                    break;
                }

                // Run the task on schedule
                await _runner.RunOnceWithRetryAsync(cancellationToken);
            }
        }
    }
}
