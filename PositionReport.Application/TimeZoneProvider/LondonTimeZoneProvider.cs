using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.TimeZoneProvider
{
    public class LondonTimeZoneProvider : ITimeZoneProvider
    {
        public TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
        }
    }
}
