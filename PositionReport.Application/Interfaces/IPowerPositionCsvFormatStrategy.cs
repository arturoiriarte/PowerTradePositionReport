using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Interfaces
{
    public interface IPowerPositionCsvFormatStrategy
    {
        /// <summary>
        /// Gets the header for the CSV file.
        /// </summary>
        /// <returns>The header content</returns>
        string GetHeader();

        /// <summary>
        /// Formats the lines of the CSV file.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>All CSV lines formatted</returns>
        IEnumerable<string> FormatLines(IDictionary<DateTime, double> data);
    }
}
