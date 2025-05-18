using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.AmbiguousTimeStrategy
{
    public class FirstUtcAmbiguousTimeStrategy : IAmbiguousTimeStrategy
    {
        public IEnumerable<DateTime> ResolveAmbiguousUtcTimes(DateTime ambiguousTime, TimeZoneInfo timeZoneInfo)
        {
            var offsets = timeZoneInfo.GetAmbiguousTimeOffsets(ambiguousTime);
            yield return ambiguousTime - offsets.First();
        }
    }
}
