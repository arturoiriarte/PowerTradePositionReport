using FluentAssertions;
using PositionReport.Application.FileNameStrategy;
using PositionReport.Application.TimeZoneProvider;
using PositionReport.Infrastructure;
using PositionReport.Infrastructure.CsvFormatStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Integration.Tests
{
    public class CsvGeneratorIntegrationTests
    {
        [Fact]
        public void ExportToCsv_ShouldCreateCorrectFile_WithContentTimestampFileNameAndISOFormat()
        {
            // Arrange
            var fileNameStrategy = new PowerPositionTimestampFileNameStrategy();
            var csvFormatStrategy = new PowerPositionCsvISO8601FormatStrategy();

            var csvGenerator = new PowerPositionSimpleCsvGenerator(
                fileNameStrategy,
                csvFormatStrategy);

            var dataToExport = simulateAggregatedData();

            var tradeDate = new DateTime(2023, 7, 2);
            var extractionUtcTimestamp = new DateTime(2023, 7, 1, 19, 15, 0);

            var fileName = fileNameStrategy.GetFileName(tradeDate, extractionUtcTimestamp);
            var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var expectedFileName = Path.Combine(filePath, fileName);

            try
            {
                // Act
                csvGenerator.GenerateCsvReportFile(dataToExport, tradeDate, extractionUtcTimestamp, filePath);

                // Assert
                fileName.Should().Be("PowerPosition_20230702_202307011915.csv");

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
            finally
            {
                // Ensure the file and directory is deleted even if the test fails
                if (File.Exists(expectedFileName)) File.Delete(expectedFileName);
                if (Directory.Exists(filePath)) Directory.Delete(filePath, true);
            }
        }

        [Fact]
        public void ExportToCsv_ShouldCreateCorrectFile_WithLocalDateTimeFileNameAndLocalTimeFormat()
        {
            // Arrange
            var berlinTimeZoneProvider = new BerlinTimeZoneProvider();
            var fileNameStrategy = new PowerPositionLocalDateTimeFileNameStrategy(berlinTimeZoneProvider);
            var csvFormatStrategy = new PowerPositionCsvLocalTimeFormatStrategy();

            var csvGenerator = new PowerPositionSimpleCsvGenerator(
                fileNameStrategy,
                csvFormatStrategy);

            var dataToExport = simulateAggregatedData();

            var tradeDate = new DateTime(2023, 7, 2);
            var extractionUtcTimestamp = new DateTime(2023, 7, 1, 19, 15, 0);
            
            var fileName = fileNameStrategy.GetFileName(tradeDate, extractionUtcTimestamp);
            var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var expectedFileName = Path.Combine(filePath, fileName);

            // Act
            csvGenerator.GenerateCsvReportFile(dataToExport, tradeDate, extractionUtcTimestamp, filePath);

            try
            {
                // Assert
                fileName.Should().Be("PowerPosition_20230701_2115.csv");

                File.Exists(expectedFileName).Should().BeTrue();

                var lines = File.ReadAllLines(expectedFileName);
                lines.Length.Should().Be(25);

                lines[0].Should().Be("Local Time;Volume");

                lines[1].Should().Be("22:00;150");
                lines[2].Should().Be("23:00;150");
                lines[3].Should().Be("00:00;150");
                lines[4].Should().Be("01:00;150");
                lines[5].Should().Be("02:00;150");
                lines[6].Should().Be("03:00;150");
                lines[7].Should().Be("04:00;150");
                lines[8].Should().Be("05:00;150");
                lines[9].Should().Be("06:00;150");
                lines[10].Should().Be("07:00;150");
                lines[11].Should().Be("08:00;150");
                lines[12].Should().Be("09:00;80");
                lines[13].Should().Be("10:00;80");
                lines[14].Should().Be("11:00;80");
                lines[15].Should().Be("12:00;80");
                lines[16].Should().Be("13:00;80");
                lines[17].Should().Be("14:00;80");
                lines[18].Should().Be("15:00;80");
                lines[19].Should().Be("16:00;80");
                lines[20].Should().Be("17:00;80");
                lines[21].Should().Be("18:00;80");
                lines[22].Should().Be("19:00;80");
                lines[23].Should().Be("20:00;80");
                lines[24].Should().Be("21:00;80");
            }
            finally
            {
                // Ensure the file is deleted even if the test fails
                if (File.Exists(expectedFileName)) File.Delete(expectedFileName);
                if (Directory.Exists(filePath)) Directory.Delete(filePath, true);
            }
        }

        private IDictionary<DateTime, double> simulateAggregatedData()
        {
            return new Dictionary<DateTime, double>
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
        }
    }
}
