using PositionReport.Application.FileNameStrategy;
using PositionReport.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Infrastructure
{
    public class PowerPositionSimpleCsvGenerator : IPowerPositionCsvGenerator
    {
        private readonly IPowerPositionFileNameStrategy _fileNameStrategy;
        private readonly IPowerPositionCsvFormatStrategy _csvFormatStrategy;
        public PowerPositionSimpleCsvGenerator(
            IPowerPositionFileNameStrategy fileNameStrategy, 
            IPowerPositionCsvFormatStrategy csvFormatStrategy)
        {
            _fileNameStrategy = fileNameStrategy;
            _csvFormatStrategy = csvFormatStrategy;
        }
        public void GenerateCsvReportFile(IDictionary<DateTime, double> data, DateTime tradeDate, DateTime extractionUtcDate, string filePath)
        {
            var fileName = _fileNameStrategy.GetFileName(tradeDate, extractionUtcDate);
            var fullPath = Path.Combine(filePath, fileName);

            Directory.CreateDirectory(filePath);

            using var writer = new StreamWriter(fullPath);
            writer.WriteLine(_csvFormatStrategy.GetHeader());

            foreach (var line in _csvFormatStrategy.FormatLines(data))
            {
                writer.WriteLine(line);
            }
        }
    }
}
