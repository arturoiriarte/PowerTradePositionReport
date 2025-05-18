using Microsoft.Extensions.Logging;
using PositionReport.Application.Interfaces;
using PositionReport.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Infrastructure
{
    public class AxpoPowerTradeServiceAdapter : IPowerTradeService
    {
        private readonly Axpo.PowerService _powerService;
        private readonly ILogger<AxpoPowerTradeServiceAdapter> _logger;
        public AxpoPowerTradeServiceAdapter(ILogger<AxpoPowerTradeServiceAdapter> logger)
        {
            _powerService = new Axpo.PowerService();
            _logger = logger;
        }

        public async Task<IEnumerable<DateTrade>> GetTradesAsync(DateTime date)
        {
            _logger.LogInformation("Calling Axpo PowerServiceAdapter");
            var trades = await _powerService.GetTradesAsync(date);

            //Could use AutoMapper here
            return trades.Select(s => new DateTrade
            {
                Date = s.Date,
                Periods = s.Periods.Select(p => new PeriodVolume
                {
                    Period = p.Period,
                    Volume = p.Volume
                })
            });
        }
    }
}
