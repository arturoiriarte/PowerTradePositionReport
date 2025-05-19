using FluentAssertions;
using PositionReport.Infrastructure.CsvFormatStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Infrastructure.Tests
{
    public class CsvFormatStrategyTests
    {
        [Fact]
        public void PowerPositionISO8601FormatStrategy_ShouldFormatCorrectly()
        {
            // Arrange
            var strategy = new PowerPositionCsvISO8601FormatStrategy();
            var data = new Dictionary<DateTime, double>
            {
                { new DateTime(2023, 7, 1, 23, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 0, 0, 0, DateTimeKind.Utc), 100 },
                { new DateTime(2023, 7, 2, 1, 0, 0, DateTimeKind.Utc), 80 }
            };

            // Act
            var header = strategy.GetHeader();
            var lines = strategy.FormatLines(data);

            // Assert
            header.Should().Be("Datetime;Volume");
            lines.Should().BeEquivalentTo(new[]
            {
                "2023-07-01T23:00:00Z;150",
                "2023-07-02T00:00:00Z;100",
                "2023-07-02T01:00:00Z;80"
            });
        }

        [Fact]
        public void PowerPositionCsvLocalTimeFormatStrategy_ShouldFormatCorrectly()
        {
            // Arrange
            var strategy = new PowerPositionCsvLocalTimeFormatStrategy();
            var data = new Dictionary<DateTime, double>
            {
                { new DateTime(2023, 7, 1, 23, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 0, 0, 0, DateTimeKind.Utc), 100 },
                { new DateTime(2023, 7, 2, 1, 0, 0, DateTimeKind.Utc), 80 }
            };

            // Act
            var header = strategy.GetHeader();
            var lines = strategy.FormatLines(data);

            // Assert
            header.Should().Be("Local Time;Volume");
            lines.Should().BeEquivalentTo(new[]
            {
                "23:00;150",
                "00:00;100",
                "01:00;80"
            });
        }
    }
}
