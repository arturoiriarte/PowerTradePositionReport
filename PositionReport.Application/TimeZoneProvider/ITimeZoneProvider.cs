using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.TimeZoneProvider
{
    public interface ITimeZoneProvider
    {
        /// <summary>
        /// Retrieves the time zone information for the trades.
        /// </summary>
        TimeZoneInfo GetTimeZone();
    }
}
