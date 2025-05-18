using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Configuration
{
    public interface IReportExportSettings
    {
        string OutputPath { get; set; }
    }
}
