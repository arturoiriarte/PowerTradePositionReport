using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Configuration
{
    public class TimeZoneSettings : ITimeZoneSettings
    {
        public string TimeZoneId { get; set; } = "Europe/Berlin";
    }
}
