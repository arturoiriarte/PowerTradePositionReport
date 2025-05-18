using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PositionReport.Application.Configuration;
using PositionReport.Application.PowerPositionScheduler;
using PositionReport.Application.TimeZoneProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.PowerPositionRunner
{
    public class PowerPositionRunnerWithMaxRetry : IPowerPositionRunnerWithRetry
    {
        private readonly ILogger<PowerPositionRunnerWithMaxRetry> _logger;
        private readonly IPowerPositionService _powerPositionService;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly ISchedulerSettings _schedulerSettings;
        private readonly IReportExportSettings _reportExportSettings;

        public PowerPositionRunnerWithMaxRetry(
            ILogger<PowerPositionRunnerWithMaxRetry> logger,
            IPowerPositionService powerPositionService,
            ITimeZoneProvider timeZoneProvider,
            IOptions<SchedulerSettings> schedulerSettings,
            IOptions<ReportExportSettings> reportExportSettings)
        {
            _logger = logger;
            _powerPositionService = powerPositionService;
            _timeZoneProvider = timeZoneProvider;
            _schedulerSettings = schedulerSettings.Value;
            _reportExportSettings = reportExportSettings.Value;
        }

        public async Task RunOnceWithRetryAsync(CancellationToken cancellationToken)
        {
            var currentUtcTimeStamp = DateTime.UtcNow;
            // Convert UTC to time zone Provider (Berlin)
            // Note: The date is set to the next day to ensure that the report is generated for the next trading day
            var date = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTimeStamp, _timeZoneProvider.GetTimeZone()).Date.AddDays(1);

            // settings
            int maxRetryAttempts = _schedulerSettings.MaxRetryAttempts;
            string filePath = _reportExportSettings.OutputPath;


            for (int i = 1; i <= maxRetryAttempts; i++)
            {
                try
                {
                    _logger.LogInformation("Processing scheduler for date: {Date:yyyy-MM-dd}", date);
                    await _powerPositionService.GeneratePowerPositionReportAsync(date, currentUtcTimeStamp, filePath);
                    _logger.LogInformation("Scheduler processed successfully for date: {Date:yyyy-MM-dd}", date);
                    break; // Exit loop if successful
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing the scheduler. Attempt {Attempt} of {MaxAttempts}", i, maxRetryAttempts);
                    if (i == maxRetryAttempts)
                    {
                        _logger.LogCritical("Max retry attempts reached.");
                    }
                    else
                    {
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // Wait before retrying
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.LogInformation("Runner operation was canceled.");
                            break; // Exit loop if the operation is canceled
                        }
                    }
                }
            }
        }
    }
}
