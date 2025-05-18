using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PositionReport.Application.Configuration;
using PositionReport.Application.PowerPositionRunner;
using PositionReport.Application.TimeZoneProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Tests
{
    public class PowerPositionRunnerWithMaxRetryTests
    {
        [Fact]
        public async Task RunOnceWithRetryAsync_ShouldRetryOnFailure()
        {
            // Arrange
            var logger = Mock.Of<ILogger<PowerPositionRunnerWithMaxRetry>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();
            var schedulerSettings = Options.Create(new SchedulerSettings() { MaxRetryAttempts = 3, TimeIntervalInMinutes = 1 });
            var reportExportSettings = Options.Create(new ReportExportSettings() { OutputPath = Path.GetTempPath() });
            var mockPowerPositionService = new Mock<IPowerPositionService>();

            mockTimeZoneProvider
                .Setup(p => p.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            var callCount = 0;
            mockPowerPositionService
                .Setup(s => s.GeneratePowerPositionReportAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    callCount++;
                    if (callCount < 3)
                    {
                        throw new Exception("Simulated failure");
                    }
                    return Task.CompletedTask;
                });

            IPowerPositionRunnerWithRetry runner = new PowerPositionRunnerWithMaxRetry(
                logger,
                mockPowerPositionService.Object,
                mockTimeZoneProvider.Object,
                schedulerSettings,
                reportExportSettings
            );

            // Act
            await runner.RunOnceWithRetryAsync(CancellationToken.None);

            // Assert
            mockPowerPositionService.Verify(s => s.GeneratePowerPositionReportAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public async Task RunOnceWithRetryAsync_ShouldLogCritical_WhenAllRetriesFail()
        {
            // Arrange
            var logger = new Mock<ILogger<PowerPositionRunnerWithMaxRetry>>();
            var mockPowerPositionService = new Mock<IPowerPositionService>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();
            var schedulerSettings = Options.Create(new SchedulerSettings() { MaxRetryAttempts = 3, TimeIntervalInMinutes = 1 });
            var reportExportSettings = Options.Create(new ReportExportSettings() { OutputPath = Path.GetTempPath() });

            mockTimeZoneProvider
                .Setup(p => p.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            mockPowerPositionService
                .Setup(s => s.GeneratePowerPositionReportAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    throw new Exception("Simulated failure");
                });

            IPowerPositionRunnerWithRetry runner = new PowerPositionRunnerWithMaxRetry(
                logger.Object,
                mockPowerPositionService.Object,
                mockTimeZoneProvider.Object,
                schedulerSettings,
                reportExportSettings
            );

            // Act
            await runner.RunOnceWithRetryAsync(CancellationToken.None);

            // Assert
            logger.Verify(l =>
                l.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Max retry attempts reached.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                )
            , Times.Once);
        }

        [Fact]
        public async Task RunOnceWithRetryAsync_ShouldStop_WhenCancellationIsRequested()
        {
            // Arrange
            var logger = new Mock<ILogger<PowerPositionRunnerWithMaxRetry>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();
            var schedulerSettings = Options.Create(new SchedulerSettings() { MaxRetryAttempts = 3, TimeIntervalInMinutes = 1 });
            var reportExportSettings = Options.Create(new ReportExportSettings() { OutputPath = Path.GetTempPath() });
            var mockPowerPositionService = new Mock<IPowerPositionService>();

            mockTimeZoneProvider
                .Setup(p => p.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            // Used so that the task can be canceled
            mockPowerPositionService
                .Setup(s => s.GeneratePowerPositionReportAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    throw new Exception("Simulated failure");
                });

            IPowerPositionRunnerWithRetry runner = new PowerPositionRunnerWithMaxRetry(
                logger.Object,
                mockPowerPositionService.Object,
                mockTimeZoneProvider.Object,
                schedulerSettings,
                reportExportSettings
            );

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(5); // Simulate cancellation.

            // Act
            var task = runner.RunOnceWithRetryAsync(cts.Token);
            await task;

            // Assert
            task.IsCompleted.Should().BeTrue();
            cts.IsCancellationRequested.Should().Be(true);
            logger.Verify(l =>
                l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Runner operation was canceled.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                )
            , Times.Once);
        }
    }
}
