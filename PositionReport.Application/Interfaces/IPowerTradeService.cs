using PositionReport.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Interfaces
{
    public interface IPowerTradeService
    {
        /// <summary>
        /// retrieves the trades for a given date.
        /// </summary>
        /// <param name="date">The date used to retrieve trade info</param>
        Task<IEnumerable<DateTrade>> GetTradesAsync(DateTime date);
    }
}
