using Microsoft.Extensions.Options;
using PositionReport.Application.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.TimeZoneProvider
{
    public class ConfigurableTimeZoneProvider : ITimeZoneProvider
    {
        private readonly ITimeZoneSettings _timeZoneSettings;

        public ConfigurableTimeZoneProvider(IOptions<TimeZoneSettings> timeZoneSettings)
        {
            _timeZoneSettings = timeZoneSettings.Value;
        }

        public TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(_timeZoneSettings.TimeZoneId);
        }
    }
}
