using PositionReport.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Infrastructure.CsvFormatStrategy
{
    public class PowerPositionCsvLocalTimeFormatStrategy : IPowerPositionCsvFormatStrategy
    {
        public string GetHeader()
        {
            return "Local Time;Volume";
        }

        public IEnumerable<string> FormatLines(IDictionary<DateTime, double> data)
        {
            return data
                .OrderBy(o => o.Key)
                .Select(kvp =>
                {
                    string localTime = kvp.Key.ToString("HH:mm");
                    string volume = kvp.Value.ToString(CultureInfo.InvariantCulture);
                    return $"{localTime};{volume}";
                });
        }
    }
}
