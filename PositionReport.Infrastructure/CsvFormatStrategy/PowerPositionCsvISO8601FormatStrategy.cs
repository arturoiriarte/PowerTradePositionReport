using PositionReport.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Infrastructure.CsvFormatStrategy
{
    public class PowerPositionCsvISO8601FormatStrategy : IPowerPositionCsvFormatStrategy
    {
        public string GetHeader()
        {
            return "Datetime;Volume";
        }

        public IEnumerable<string> FormatLines(IDictionary<DateTime, double> data)
        {
            return data
                .OrderBy(o => o.Key)
                .Select (kvp =>
                {
                    // Format the date as ISO 8601
                    string formattedDate = kvp.Key.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    string volume = kvp.Value.ToString(CultureInfo.InvariantCulture);
                    return $"{formattedDate};{volume}";
                });
        }    
    }
}
