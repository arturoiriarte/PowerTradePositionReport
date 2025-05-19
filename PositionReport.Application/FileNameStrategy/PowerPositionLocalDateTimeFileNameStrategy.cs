using PositionReport.Application.TimeZoneProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.FileNameStrategy
{
    public class PowerPositionLocalDateTimeFileNameStrategy : IPowerPositionFileNameStrategy
    {
        private readonly ITimeZoneProvider _timeZoneProvider;
        public PowerPositionLocalDateTimeFileNameStrategy(ITimeZoneProvider timeZoneProvider)
        {
            _timeZoneProvider = timeZoneProvider;
        }

        public string GetFileName(DateTime tradeDate, DateTime extractionUtcDate)
        {
            var timeZone = _timeZoneProvider.GetTimeZone();
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(extractionUtcDate, timeZone);
            return $"PowerPosition_{localDateTime:yyyyMMdd_HHmm}.csv";
        }
    }
}
