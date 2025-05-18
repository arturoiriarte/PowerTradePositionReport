using PositionReport.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.PowerTradeAggregator
{
    public interface IPowerTradeAggregator
    {
        /// <summary>
        /// Aggregates the trades for a given date and returns the aggregated position.
        /// </summary>
        /// <param name="trades">The Enumerable DateTrade used to aggregate data</param>
        Task<Dictionary<DateTime, double>> GetAggregatedPositionAsync(IEnumerable<DateTrade> trades);
    }
}
