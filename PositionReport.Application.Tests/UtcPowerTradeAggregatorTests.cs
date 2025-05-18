using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PositionReport.Application.AmbiguousTimeStrategy;
using PositionReport.Application.PowerTradeAggregator;
using PositionReport.Application.TimeZoneProvider;
using PositionReport.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Tests
{
    public class UtcPowerTradeAggregatorTests
    {
        [Fact]
        public async Task GetAggregatedPositionAsync_ShouldGroupAndSumTradesByHour()
        {
            // Arrange
            var tradeDate = new DateTime(2023, 7, 2);
            var trades = new List<DateTrade>
            {
                new DateTrade
                {
                    Date = tradeDate,
                    Periods = new List<PeriodVolume>
                    {
                        new PeriodVolume { Period = 1, Volume = 10 },
                        new PeriodVolume { Period = 2, Volume = 20 },
                        new PeriodVolume { Period = 3, Volume = 10 }
                    }
                },
                new DateTrade
                {
                    Date = tradeDate,
                    Periods = new List<PeriodVolume>
                    {
                        new PeriodVolume { Period = 1, Volume = 5 },
                        new PeriodVolume { Period = 2, Volume = 15 },
                        new PeriodVolume { Period = 3, Volume = 10 }
                    }
                }
            };

            var logger = Mock.Of<ILogger<UtcPowerTradeAggregator>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();
            var ambiguousTimeStrategy = Mock.Of<IAmbiguousTimeStrategy>();

            mockTimeZoneProvider
                .Setup(x => x.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            var service = new UtcPowerTradeAggregator(logger, mockTimeZoneProvider.Object, ambiguousTimeStrategy);

            // Act
            var result = await service.GetAggregatedPositionAsync(trades);

            // Assert
            result.Should().HaveCount(3);

            result.Should().ContainKey(new DateTime(2023, 7, 1, 22, 0, 0, DateTimeKind.Utc));
            result[new DateTime(2023, 7, 1, 22, 0, 0, DateTimeKind.Utc)].Should().Be(15);

            result.Should().ContainKey(new DateTime(2023, 7, 1, 23, 0, 0, DateTimeKind.Utc));
            result[new DateTime(2023, 7, 1, 23, 0, 0, DateTimeKind.Utc)].Should().Be(35);

            result.Should().ContainKey(new DateTime(2023, 7, 2, 0, 0, 0, DateTimeKind.Utc));
            result[new DateTime(2023, 7, 2, 0, 0, 0, DateTimeKind.Utc)].Should().Be(20);
        }

        [Fact]
        public async Task GetAggregatedPositionAsync_ShouldReturnEmpty_WhenNoTrades()
        {
            // Arrange
            var tradeDate = new DateTime(2023, 7, 2);
            var trades = new List<DateTrade>();

            var logger = Mock.Of<ILogger<UtcPowerTradeAggregator>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();
            var ambiguousTimeStrategy = Mock.Of<IAmbiguousTimeStrategy>();

            mockTimeZoneProvider
                .Setup(x => x.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            var service = new UtcPowerTradeAggregator(logger, mockTimeZoneProvider.Object, ambiguousTimeStrategy);

            // Act
            var result = await service.GetAggregatedPositionAsync(trades);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAggregatedPositionAsync_ShouldHandleDstStartTransition_Correctly()
        {
            // Arrange
            var tradeDate = new DateTime(2023, 3, 26); // DST start date in 2023

            var trades = new List<DateTrade>
            {
                new DateTrade
                {
                    Date = tradeDate,
                    Periods = Enumerable.Range(1, 24).Select(i => new PeriodVolume
                    {
                        Period = i,
                        Volume = 1
                    }).ToList()
                }
            };

            var logger = Mock.Of<ILogger<UtcPowerTradeAggregator>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();
            var ambiguousTimeStrategy = Mock.Of<IAmbiguousTimeStrategy>();

            mockTimeZoneProvider
                .Setup(x => x.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            var aggregator = new UtcPowerTradeAggregator(logger, mockTimeZoneProvider.Object, ambiguousTimeStrategy);

            // Act
            var result = await aggregator.GetAggregatedPositionAsync(trades);

            // Assert
            result.Should().HaveCount(23); // Only 23 real UTC hours on DST start day
            result.Values.Sum().Should().Be(23); // 1 volume skipped due to DST
        }

        [Fact]
        public async Task GetAggregatedPositionAsync_ShouldHandleDstEndTransition_Correctly_WhenAllUtcAmbiguousTimeStrategy()
        {
            // Arrange
            var tradeDate = new DateTime(2023, 10, 29); // DST end date in 2023

            var trades = new List<DateTrade>
            {
                new DateTrade
                {
                    Date = tradeDate,
                    Periods = Enumerable.Range(1, 24).Select(i => new PeriodVolume
                    {
                        Period = i,
                        Volume = 1
                    }).ToList()
                }
            };

            var logger = Mock.Of<ILogger<UtcPowerTradeAggregator>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();

            mockTimeZoneProvider
                .Setup(x => x.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            var aggregator = new UtcPowerTradeAggregator(logger, mockTimeZoneProvider.Object, new FirstUtcAmbiguousTimeStrategy());

            // Act
            var result = await aggregator.GetAggregatedPositionAsync(trades);

            // Assert
            result.Should().HaveCount(24); // DST End have 25 real UTC hours. 1 hour is ambiguous. Using FirstUtcAmbiguousTimeStrategy 
            result.Values.Sum().Should().Be(24); // Not duplicating the volume
        }

        [Fact]
        public async Task GetAggregatedPositionAsync_ShouldHandleDstEndTransition_Correctly_WhenFirstUtcAmbiguousTimeStrategy()
        {
            // Arrange
            var tradeDate = new DateTime(2023, 10, 29); // DST end date in 2023

            var trades = new List<DateTrade>
            {
                new DateTrade
                {
                    Date = tradeDate,
                    Periods = Enumerable.Range(1, 24).Select(i => new PeriodVolume
                    {
                        Period = i,
                        Volume = 1
                    }).ToList()
                }
            };

            var logger = Mock.Of<ILogger<UtcPowerTradeAggregator>>();
            var mockTimeZoneProvider = new Mock<ITimeZoneProvider>();

            mockTimeZoneProvider
                .Setup(x => x.GetTimeZone())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            var aggregator = new UtcPowerTradeAggregator(logger, mockTimeZoneProvider.Object, new AllUtcAmbiguousTimeStrategy());

            // Act
            var result = await aggregator.GetAggregatedPositionAsync(trades);

            // Assert
            result.Should().HaveCount(25); // DST End have 25 real UTC hours. 1 hour is ambiguous. Using AllUtcAmbiguousTimeStrategy
            result.Values.Sum().Should().Be(25); // Taking the volume twice
        }
    }
}
