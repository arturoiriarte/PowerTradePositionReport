using PositionReport.Application.Interfaces;
using PositionReport.Application.PowerTradeAggregator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application
{
    public class PowerPositionService : IPowerPositionService
    {
        private readonly IPowerTradeService _powerTradeService;
        private readonly IPowerTradeAggregator _tradeAggregator;
        private readonly IPowerPositionCsvGenerator _csvGenerator;

        public PowerPositionService(
             IPowerTradeService powerTradeService, 
             IPowerTradeAggregator tradeAggregator, 
             IPowerPositionCsvGenerator csvGenerator)
        {
            _powerTradeService = powerTradeService;
            _tradeAggregator = tradeAggregator;
            _csvGenerator = csvGenerator;
        }

        public async Task GeneratePowerPositionReportAsync(DateTime tradeDate, DateTime extractionUtcTimestamp, string filePath)
        {
            var trades = await _powerTradeService.GetTradesAsync(tradeDate);
            var aggregatedPositions = await _tradeAggregator.GetAggregatedPositionAsync(trades);
            _csvGenerator.GenerateCsvReportFile(aggregatedPositions, tradeDate, extractionUtcTimestamp, filePath);
        }
    }
}
