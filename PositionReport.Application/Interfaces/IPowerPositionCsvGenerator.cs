using PositionReport.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Interfaces
{
    public interface IPowerPositionCsvGenerator
    {
        /// <summary>
        /// Generates a CSV file from the given data and saves it to the specified path.
        /// </summary>
        /// <param name="data">The data to be converted to CSV format.</param>
        /// <param name="tradeDate">The date of the trade volumes</param>
        /// <param name="extractionUtcTimestamp">The timestamp of the extraction in UTC</param>
        /// <param name="filePath">The path where the CSV file will be saved.</param>
        void GenerateCsvReportFile(IDictionary<DateTime, double> data, DateTime tradeDate, DateTime extractionUtcTimestamp, string filePath);
    }
}
