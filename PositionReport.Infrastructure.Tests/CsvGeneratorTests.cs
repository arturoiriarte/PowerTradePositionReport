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
            var fileName = "test.csv";
            var mockFileNameStrategy = new Mock<IPowerPositionFileNameStrategy>();
            mockFileNameStrategy
                .Setup(s => s.GetFileName(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(fileName);

            var mockCsvFormatStrategy = new Mock<IPowerPositionCsvFormatStrategy>();
            mockCsvFormatStrategy
                .Setup(s => s.GetHeader())
                .Returns("Column1;Column2");

            mockCsvFormatStrategy
                .Setup(s => s.FormatLines(It.IsAny<IDictionary<DateTime, double>>()))
                .Returns(Enumerable.Range(1, 2).Select(i => $"{i};{i}"));

            IPowerPositionCsvGenerator csvGenerator = new PowerPositionSimpleCsvGenerator(
                mockFileNameStrategy.Object, 
                mockCsvFormatStrategy.Object);

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var expectedFileName = Path.Combine(tempPath, fileName);

            if (File.Exists(expectedFileName))
                File.Delete(expectedFileName);

            try
            {
                // Act
                csvGenerator.GenerateCsvReportFile(It.IsAny<IDictionary<DateTime, double>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), tempPath);

                // Assert
                File.Exists(expectedFileName).Should().BeTrue();

                var lines = File.ReadAllLines(expectedFileName);
                lines.Length.Should().Be(3);

                lines[0].Should().Be("Column1;Column2");
                lines[1].Should().Be("1;1");
                lines[2].Should().Be("2;2");
            }
            finally
            {
                // Ensure the file and directory is deleted even if the test fails
                if (File.Exists(expectedFileName)) File.Delete(expectedFileName);
                if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void ExportToCsv_ShouldCreateCorrectFile_WithCorrectContent_EmptyData()
        {
            // Arrange
            var fileName = "test.csv";
            var mockFileNameStrategy = new Mock<IPowerPositionFileNameStrategy>();
            mockFileNameStrategy
                .Setup(s => s.GetFileName(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(fileName);

            var mockCsvFormatStrategy = new Mock<IPowerPositionCsvFormatStrategy>();
            mockCsvFormatStrategy
                .Setup(s => s.GetHeader())
                .Returns("Column1;Column2");

            IPowerPositionCsvGenerator csvGenerator = new PowerPositionSimpleCsvGenerator(
                mockFileNameStrategy.Object, 
                mockCsvFormatStrategy.Object);

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var expectedFileName = Path.Combine(tempPath, fileName);

            if (File.Exists(expectedFileName))
                File.Delete(expectedFileName);

            try
            {
                // Act
                csvGenerator.GenerateCsvReportFile(It.IsAny<IDictionary<DateTime, double>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), tempPath);

                // Assert
                File.Exists(expectedFileName).Should().BeTrue();
                var lines = File.ReadAllLines(expectedFileName);
                lines.Length.Should().Be(1);
                lines[0].Should().Be("Column1;Column2");
            }
            finally
            {
                // Ensure the file and directory is deleted even if the test fails
                if (File.Exists(expectedFileName)) File.Delete(expectedFileName);
                if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            }
        }
    }
}
