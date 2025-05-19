using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.FileNameStrategy
{
    public interface IPowerPositionFileNameStrategy
    {
        /// <summary>
        /// Get the file name for the Power Position report based on the trade date and extraction UTC date.
        /// </summary>
        /// <param name="tradeDate">The date of the trade volumes</param>
        /// <param name="extractionUtcDate">The timestamp of the extraction in UTC</param>
        /// <returns></returns>
        public string GetFileName(DateTime tradeDate, DateTime extractionUtcDate);
    }
}
