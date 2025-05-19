using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.FileNameStrategy
{
    public interface IPowerPositionFileNameStrategy
    {
        public string GetFileName(DateTime tradeDate, DateTime extractionUtcDate);
    }
}
