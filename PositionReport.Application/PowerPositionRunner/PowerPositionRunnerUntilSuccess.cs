using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PositionReport.Application.Configuration;
using PositionReport.Application.TimeZoneProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.PowerPositionRunner
{
    public class PowerPositionRunnerUntilSuccess : IPowerPositionRunnerWithRetry
    {
        private readonly ILogger<PowerPositionRunnerUntilSuccess> _logger;
        private readonly IPowerPositionService _powerPositionService;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly IReportExportSettings _reportExportSettings;

        public PowerPositionRunnerUntilSuccess(
            ILogger<PowerPositionRunnerUntilSuccess> logger,
            IPowerPositionService powerPositionService,
            ITimeZoneProvider timeZoneProvider,
            IOptions<ReportExportSettings> reportExportSettings)
        {
            _logger = logger;
            _powerPositionService = powerPositionService;
            _timeZoneProvider = timeZoneProvider;
            _reportExportSettings = reportExportSettings.Value;
        }

        public async Task RunOnceWithRetryAsync(CancellationToken cancellationToken)
        {
            var currentUtcTimeStamp = DateTime.UtcNow;
            // Convert UTC to time zone Provider (Berlin)
            // Note: The date is set to the next day to ensure that the report is generated for the next trading day
            var date = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTimeStamp, _timeZoneProvider.GetTimeZone()).Date.AddDays(1);

            // settings
            string filePath = _reportExportSettings.OutputPath;

            var callCount = 0;

            while (true)
            {
                callCount++;
                try
                {
                    _logger.LogInformation("Processing scheduler for date: {Date:yyyy-MM-dd}", date);
                    await _powerPositionService.GeneratePowerPositionReportAsync(date, currentUtcTimeStamp, filePath);
                    _logger.LogInformation("Scheduler processed successfully for date: {Date:yyyy-MM-dd}", date);
                    break; // Exit loop if successful
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing the scheduler. Attempt {Attempt}", callCount);
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
