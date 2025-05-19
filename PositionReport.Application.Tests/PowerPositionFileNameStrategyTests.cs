using FluentAssertions;
using Moq;
using PositionReport.Application.FileNameStrategy;
using PositionReport.Application.TimeZoneProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Tests
{
    public class PowerPositionFileNameStrategyTests
    {
        [Fact]
        public void GetFileName_ShouldReturnCorrectFileName_WhenPowerPositionTimestampFileNameStrategy()
        {
            // Arrange
            var strategy = new PowerPositionTimestampFileNameStrategy();
            var tradeDate = new DateTime(2023, 7, 2);
            var extractionUtcTimestamp = new DateTime(2023, 7, 1, 19, 15, 0);

            // Act
            var fileName = strategy.GetFileName(tradeDate, extractionUtcTimestamp);

            // Assert
            fileName.Should().Be("PowerPosition_20230702_202307011915.csv");
        }

        [Fact]
        public void GetFileName_ShouldReturnCorrectFileName_WhenPowerPositionLocalDateTimeFileNameStrategy()
        {
            // Arrange
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();

            mockTimeZoneProvider
                .Setup(p => p.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            var strategy = new PowerPositionLocalDateTimeFileNameStrategy(mockTimeZoneProvider.Object);
            var tradeDate = new DateTime(2023, 7, 2);
            var extractionUtcTimestamp = new DateTime(2023, 7, 1, 19, 15, 0);

            // Act
            var fileName = strategy.GetFileName(tradeDate, extractionUtcTimestamp);

            // Assert
            fileName.Should().Be("PowerPosition_20230701_2115.csv");
        }
    }
}
