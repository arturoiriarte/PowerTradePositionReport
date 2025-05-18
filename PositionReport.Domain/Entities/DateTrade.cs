using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Domain.Entities
{
    public class DateTrade
    {
        public DateTime Date { get; set; }
        public IEnumerable<PeriodVolume> Periods { get; set; } = Enumerable.Empty<PeriodVolume>();
    }
}
