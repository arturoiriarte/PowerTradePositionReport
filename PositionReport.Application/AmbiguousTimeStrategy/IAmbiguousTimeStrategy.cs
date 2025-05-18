using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.AmbiguousTimeStrategy
{
    public interface IAmbiguousTimeStrategy
    {
        /// <summary>
        /// Resolves ambiguous UTC times by generating a list of possible UTC times based on the provided ambiguous time and time zone information.
        /// </summary>
        /// <param name="ambiguousTime">The ambiguous time to be converted to UTC</param>
        /// <param name="timeZoneInfo">The time zone used to get time offsets</param>
        IEnumerable<DateTime> ResolveAmbiguousUtcTimes(DateTime ambiguousTime, TimeZoneInfo timeZoneInfo);    
    }
}
