using FluentAssertions;
using Moq;
using PositionReport.Application.FileNameStrategy;
using PositionReport.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Infrastructure.Tests
{
    public class CsvGeneratorTests
    {
        [Fact]
        public void ExportToCsv_ShouldCreateCorrectFile_WithCorrectContent()
        {
            // Arrange
            var mockFileNameStrategy = new Mock<IPowerPositionFileNameStrategy>();
            mockFileNameStrategy
                .Setup(s => s.GetFileName(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns("PowerPosition_20230702_202307011915.csv");

            IPowerPositionCsvGenerator csvGenerator = new PowerPositionSimpleCsvGenerator(mockFileNameStrategy.Object);
            IDictionary<DateTime, double> dataToExport = new Dictionary<DateTime, double>
            {
                { new DateTime(2023, 7, 1, 22, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 1, 23, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 0, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 1, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 2, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 3, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 4, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 5, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 6, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 7, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 8, 0, 0, DateTimeKind.Utc), 150 },
                { new DateTime(2023, 7, 2, 9, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 10, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 11, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 12, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 13, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 14, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 15, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 16, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 17, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 18, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 19, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 20, 0, 0, DateTimeKind.Utc), 80 },
                { new DateTime(2023, 7, 2, 21, 0, 0, DateTimeKind.Utc), 80 },
            };

            var tradeDate = new DateTime(2023, 7, 2);
            var extractionUtcTimestamp = new DateTime(2023, 7, 1, 19, 15, 0);

            var filePath = Path.GetTempPath();
            var expectedFileName = Path.Combine(filePath, "PowerPosition_20230702_202307011915.csv");

            if (File.Exists(expectedFileName))
                File.Delete(expectedFileName);

            // Act
            csvGenerator.GenerateCsvReportFile(dataToExport, tradeDate, extractionUtcTimestamp, filePath);

            // Assert
            File.Exists(expectedFileName).Should().BeTrue();

            var lines = File.ReadAllLines(expectedFileName);
            lines.Length.Should().Be(25);

            lines[0].Should().Be("Datetime;Volume");

            lines[1].Should().Be("2023-07-01T22:00:00Z;150");
            lines[2].Should().Be("2023-07-01T23:00:00Z;150");
            lines[3].Should().Be("2023-07-02T00:00:00Z;150");
            lines[4].Should().Be("2023-07-02T01:00:00Z;150");
            lines[5].Should().Be("2023-07-02T02:00:00Z;150");
            lines[6].Should().Be("2023-07-02T03:00:00Z;150");
            lines[7].Should().Be("2023-07-02T04:00:00Z;150");
            lines[8].Should().Be("2023-07-02T05:00:00Z;150");
            lines[9].Should().Be("2023-07-02T06:00:00Z;150");
            lines[10].Should().Be("2023-07-02T07:00:00Z;150");
            lines[11].Should().Be("2023-07-02T08:00:00Z;150");
            lines[12].Should().Be("2023-07-02T09:00:00Z;80");
            lines[13].Should().Be("2023-07-02T10:00:00Z;80");
            lines[14].Should().Be("2023-07-02T11:00:00Z;80");
            lines[15].Should().Be("2023-07-02T12:00:00Z;80");
            lines[16].Should().Be("2023-07-02T13:00:00Z;80");
            lines[17].Should().Be("2023-07-02T14:00:00Z;80");
            lines[18].Should().Be("2023-07-02T15:00:00Z;80");
            lines[19].Should().Be("2023-07-02T16:00:00Z;80");
            lines[20].Should().Be("2023-07-02T17:00:00Z;80");
            lines[21].Should().Be("2023-07-02T18:00:00Z;80");
            lines[22].Should().Be("2023-07-02T19:00:00Z;80");
            lines[23].Should().Be("2023-07-02T20:00:00Z;80");
            lines[24].Should().Be("2023-07-02T21:00:00Z;80");

            // Clean up
            File.Delete(expectedFileName);
        }

        [Fact]
        public void ExportToCsv_ShouldCreateCorrectFile_WithCorrectContent_EmptyData()
        {
            // Arrange
            var mockFileNameStrategy = new Mock<IPowerPositionFileNameStrategy>();
            mockFileNameStrategy
                .Setup(s => s.GetFileName(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns("PowerPosition_20230702_202307011915.csv");

            IPowerPositionCsvGenerator csvGenerator = new PowerPositionSimpleCsvGenerator(mockFileNameStrategy.Object);
            IDictionary<DateTime, double> dataToExport = new Dictionary<DateTime, double>();
            var tradeDate = new DateTime(2023, 7, 2);
            var extractionUtcTimestamp = new DateTime(2023, 7, 1, 19, 15, 0);
            var filePath = Path.GetTempPath();
            var expectedFileName = Path.Combine(filePath, "PowerPosition_20230702_202307011915.csv");

            if (File.Exists(expectedFileName))
                File.Delete(expectedFileName);

            // Act
            csvGenerator.GenerateCsvReportFile(dataToExport, tradeDate, extractionUtcTimestamp, filePath);

            // Assert
            File.Exists(expectedFileName).Should().BeTrue();
            var lines = File.ReadAllLines(expectedFileName);
            lines.Length.Should().Be(1);
            lines[0].Should().Be("Datetime;Volume");

            // Clean up
            File.Delete(expectedFileName);
        }

        [Theory]
        [InlineData("20230702", "202307011915", "PowerPosition_20230702_202307011915.csv")]
        [InlineData("20240227", "202402261005", "PowerPosition_20240227_202402261005.csv")]
        [InlineData("20250517", "202505162227", "PowerPosition_20250517_202505162227.csv")]
        public void ExportToCsv_ShouldCreateFileWithExpectedFileName(string tradeDateStr, string extractionUtcTimestampStr, string fileName)
        {
            // Arrange
            var mockFileNameStrategy = new Mock<IPowerPositionFileNameStrategy>();
            mockFileNameStrategy
                .Setup(s => s.GetFileName(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns($"PowerPosition_{tradeDateStr}_{extractionUtcTimestampStr}.csv");

            IPowerPositionCsvGenerator csvGenerator = new PowerPositionSimpleCsvGenerator(mockFileNameStrategy.Object);
            IDictionary<DateTime, double> dataToExport = new Dictionary<DateTime, double>();
            var filePath = Path.GetTempPath();
            var expectedFileName = Path.Combine(filePath, fileName);

            DateTime tradeDate = DateTime.ParseExact(tradeDateStr, "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime extractionUtcTimestamp = DateTime.ParseExact(extractionUtcTimestampStr, "yyyyMMddHHmm", CultureInfo.InvariantCulture);

            if (File.Exists(expectedFileName))
                File.Delete(expectedFileName);

            // Act
            csvGenerator.GenerateCsvReportFile(dataToExport, tradeDate, extractionUtcTimestamp, filePath);

            // Assert
            File.Exists(expectedFileName).Should().BeTrue();
            Path.GetFileName(expectedFileName).Should().Be(fileName);

            // Clean up
            File.Delete(expectedFileName);
        }
    }
}
