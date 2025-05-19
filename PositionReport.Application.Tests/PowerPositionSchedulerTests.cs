using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PositionReport.Application.Configuration;
using PositionReport.Application.PowerPositionRunner;
using PositionReport.Application.PowerPositionScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Tests
{
    public class PowerPositionSimpleSchedulerRunOnceTests
    {
        [Fact]
        public async Task RunAsync_ShouldCallRunnerImmediately_WhenStarted()
        {
            // Arrange
            var logger = Mock.Of<ILogger<PowerPositionSimpleSchedulerRunImmediately>>();
            var schedulerSettings = Options.Create(new SchedulerSettings() { MaxRetryAttempts = 3, TimeIntervalInMinutes = 5 });
            var mockPowerPositionRunnerWithRetry = new Mock<IPowerPositionRunnerWithRetry>();

            var scheduler = new PowerPositionSimpleSchedulerRunImmediately(logger, schedulerSettings, mockPowerPositionRunnerWithRetry.Object);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(60); // Simulate cancellation after 60ms. Needed to avoid infinite loop in the test.

            // Act
            var task = scheduler.RunAsync(cts.Token);
            await task;

            // Assert
            task.IsCompleted.Should().BeTrue();
            mockPowerPositionRunnerWithRetry.Verify(s => s.RunOnceWithRetryAsync(cts.Token), Times.Once);
        }

        // Notes: We can improve the test duration by using a smaller time interval in the scheduler settings.
        // Consider change TimeIntervalInMinutes to TimeIntervalInSeconds or TimeIntervalInMilliseconds to reduce the wait time.
        // This would require changes in the SchedulerSettings class and the PowerPositionSimpleSchedulerRunImmediately class.
        // An Alternative workaround would be to change type from int to double in order to allow fractions of a minute.
        [Fact]
        public async Task RunAsync_ShouldRunMultipleTimes_WhenNotCancelled()
        {
            // Arrange
            var logger = Mock.Of<ILogger<PowerPositionSimpleSchedulerRunImmediately>>();
            var schedulerSettings = Options.Create(new SchedulerSettings() { MaxRetryAttempts = 3, TimeIntervalInMinutes = 1 });
            var mockPowerPositionRunnerWithRetry = new Mock<IPowerPositionRunnerWithRetry>();

            var scheduler = new PowerPositionSimpleSchedulerRunImmediately(logger, schedulerSettings, mockPowerPositionRunnerWithRetry.Object);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMinutes(2)); // Simulate cancellation after 2min. Needed to avoid infinite loop in the test.

            // Act
            var task = scheduler.RunAsync(cts.Token);
            await task;

            // Assert
            task.IsCompleted.Should().BeTrue();
            mockPowerPositionRunnerWithRetry.Verify(s => s.RunOnceWithRetryAsync(cts.Token), Times.AtLeast(2));
        }

        [Fact]
        public async Task RunAsync_ShouldStop_WhenCancellationIsRequested()
        {
            // Arrange
            var logger = Mock.Of<ILogger<PowerPositionSimpleSchedulerRunImmediately>>();
            var schedulerSettings = Options.Create(new SchedulerSettings() { MaxRetryAttempts = 3, TimeIntervalInMinutes = 5 });
            var mockPowerPositionRunnerWithRetry = new Mock<IPowerPositionRunnerWithRetry>();

            var scheduler = new PowerPositionSimpleSchedulerRunImmediately(logger, schedulerSettings, mockPowerPositionRunnerWithRetry.Object);

            using var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately to simulate cancellation. Should run only once.

            // Act
            var task = scheduler.RunAsync(cts.Token);
            await task;

            // Assert
            task.IsCompleted.Should().BeTrue();
            mockPowerPositionRunnerWithRetry.Verify(s => s.RunOnceWithRetryAsync(cts.Token), Times.Once);
            cts.IsCancellationRequested.Should().Be(true);
        }
    }
}
