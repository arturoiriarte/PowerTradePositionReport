using FluentAssertions;
using PositionReport.Application.AmbiguousTimeStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Tests
{
    public class AmbiguousTimeStrategyTests
    {
        [Fact]
        public void ResolveAmbiguousUtcTimes_ShouldReturnOneUtcTime_WhenUsingFirstUtcAmbiguousTimeStrategy()
        {
            // Arrange
            var ambiguousTime = new DateTime(2023, 10, 29, 2, 0, 0, DateTimeKind.Unspecified);
            var strategy = new FirstUtcAmbiguousTimeStrategy();

            // Act
            var result = strategy.ResolveAmbiguousUtcTimes(ambiguousTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            // Assert
            result.Should().HaveCount(1);
            result.First().Should().Be(new DateTime(2023, 10, 29, 1, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void ResolveAmbiguousUtcTimes_ShouldReturnAllUtcTime_WhenUsingAllUtcAmbiguousTimeStrategy()
        {
            // Arrange
            var ambiguousTime = new DateTime(2023, 10, 29, 2, 0, 0, DateTimeKind.Unspecified);
            var strategy = new AllUtcAmbiguousTimeStrategy();

            // Act
            var result = strategy.ResolveAmbiguousUtcTimes(ambiguousTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));

            // Assert
            result.Should().HaveCount(2);
            result.ElementAt(0).Should().Be(new DateTime(2023, 10, 29, 1, 0, 0, DateTimeKind.Utc));
            result.ElementAt(1).Should().Be(new DateTime(2023, 10, 29, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
