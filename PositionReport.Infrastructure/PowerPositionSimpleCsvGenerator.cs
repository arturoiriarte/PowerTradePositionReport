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
        public PowerPositionSimpleCsvGenerator(IPowerPositionFileNameStrategy fileNameStrategy)
        {
            _fileNameStrategy = fileNameStrategy;
        }
        public void GenerateCsvReportFile(IDictionary<DateTime, double> data, DateTime tradeDate, DateTime extractionUtcDate, string filePath)
        {
            var fileName = _fileNameStrategy.GetFileName(tradeDate, extractionUtcDate);
            var fullPath = Path.Combine(filePath, fileName);

            Directory.CreateDirectory(filePath);

            using var writer = new StreamWriter(fullPath);
            writer.WriteLine("Datetime;Volume");
            foreach (var kvp in data.OrderBy(o => o.Key))
            {
                // Format the date as ISO 8601
                string formattedDate = kvp.Key.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string volume = kvp.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                writer.WriteLine($"{formattedDate};{volume}");
            }
        }
    }
}
