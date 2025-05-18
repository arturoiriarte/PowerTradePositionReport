using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application
{
    public interface IPowerPositionService
    {
        /// <summary>
        /// Orchestrates the process of generating a power position report.
        /// </summary>
        /// <param name="tradeDate">The day of the volumes (Day-ahead)</param>
        /// <param name="extractionUtcTimestamp">The UTC timestamp of the power trade position extraction</param>
        /// <param name="filePath">The filepath to use for report storing</param>
        Task GeneratePowerPositionReportAsync(DateTime tradeDate, DateTime extractionUtcTimestamp, string filePath);
    }
}
