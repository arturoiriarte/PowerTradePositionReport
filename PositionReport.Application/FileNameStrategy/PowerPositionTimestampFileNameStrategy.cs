using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.FileNameStrategy
{
    public class PowerPositionTimestampFileNameStrategy : IPowerPositionFileNameStrategy
    {
        public string GetFileName(DateTime tradeDate, DateTime extractionUtcDate)
        {
            return $"PowerPosition_{tradeDate:yyyyMMdd}_{extractionUtcDate:yyyyMMddHHmm}.csv";
        }
    }
}
