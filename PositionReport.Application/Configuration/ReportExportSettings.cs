using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Configuration
{
    public class ReportExportSettings : IReportExportSettings
    {
        public string OutputPath { get; set; } = "./reports";
    }
}
