using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Configuration
{
    public interface ISchedulerSettings
    {
        int TimeIntervalInMinutes { get; set; }
        int MaxRetryAttempts { get; set; }
    }
}
