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
        public void GenerateCsvReportFile(IDictionary<DateTime, double> data, DateTime tradeDate, DateTime extractionUtcDate, string filePath)
        {
            var filename = $"PowerPosition_{tradeDate:yyyyMMdd}_{extractionUtcDate:yyyyMMddHHmm}.csv";
            var fullPath = Path.Combine(filePath, filename);

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
