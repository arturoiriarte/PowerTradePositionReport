using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Configuration
{
    public class SchedulerSettings : ISchedulerSettings
    {
        public int TimeIntervalInMinutes { get; set; } = 5;
        public int MaxRetryAttempts { get; set; } = 3;
    }
}
