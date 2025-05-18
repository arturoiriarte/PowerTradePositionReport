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
    public class PowerPositionRunnerRunUntilSuccessTests
    {
        [Fact]
        public async Task RunOnceWithRetryAsync_ShouldRetryOnFailure()
        {
            // Arrange
            var logger = new Mock<ILogger<PowerPositionRunnerUntilSuccess>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();
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

            IPowerPositionRunnerWithRetry runner = new PowerPositionRunnerUntilSuccess(
                logger.Object,
                mockPowerPositionService.Object,
                mockTimeZoneProvider.Object,
                reportExportSettings
            );

            // Act
            await runner.RunOnceWithRetryAsync(CancellationToken.None);

            // Assert
            mockPowerPositionService.Verify(s => s.GeneratePowerPositionReportAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Exactly(3));
            logger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error occurred while processing the scheduler. Attempt 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                )
            , Times.Once);

            logger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error occurred while processing the scheduler. Attempt 2")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                )
            , Times.Once);

        }

        [Fact]
        public async Task RunOnceWithRetryAsync_ShouldStop_WhenCancellationIsRequested()
        {
            // Arrange
            var logger = new Mock<ILogger<PowerPositionRunnerUntilSuccess>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();
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
                    throw new Exception("Simulated failure");
                });

            IPowerPositionRunnerWithRetry runner = new PowerPositionRunnerUntilSuccess(
                logger.Object,
                mockPowerPositionService.Object,
                mockTimeZoneProvider.Object,
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
