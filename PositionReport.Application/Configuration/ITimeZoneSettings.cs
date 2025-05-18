using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Configuration
{
    public interface ITimeZoneSettings
    {
        string TimeZoneId { get; set; }
    }
}
