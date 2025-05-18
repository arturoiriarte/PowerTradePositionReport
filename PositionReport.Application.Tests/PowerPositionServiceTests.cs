using Moq;
using PositionReport.Application.Interfaces;
using PositionReport.Application.PowerTradeAggregator;
using PositionReport.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Tests
{
    public class PowerPositionServiceTests
    {
        [Fact]
        public async Task GeneratePowerPositionReportAsync_ShouldCallDependencies_AndGenerateReport()
        {
            // Arrange
            var tradeDate = new DateTime(2023, 7, 2);
            var extractionUtcTimestamp = new DateTime(2023, 7, 1, 19, 15, 0);
            var filePath = Path.GetTempPath();

            var mockTradeService = new Mock<IPowerTradeService>();
            var mockAggregator = new Mock<IPowerTradeAggregator>();
            var mockCsvGenerator = new Mock<IPowerPositionCsvGenerator>();

            var trades = new List<DateTrade>
            {
                new DateTrade
                {
                    Date = tradeDate,
                    Periods = new List<PeriodVolume>
                    {
                        new PeriodVolume { Period = 1, Volume = 100 },
                        new PeriodVolume { Period = 2, Volume = 200 }
                    }
                }
            };

            var aggregatedPositions = new Dictionary<DateTime, double>
            {
                { new DateTime(2023, 7, 2, 0, 0, 0, DateTimeKind.Utc), 100 },
                { new DateTime(2023, 7, 2, 1, 0, 0, DateTimeKind.Utc), 200 }
            };

            mockTradeService
                .Setup(s => s.GetTradesAsync(tradeDate))
                .ReturnsAsync(trades);

            mockAggregator
                .Setup(s => s.GetAggregatedPositionAsync(trades))
                .ReturnsAsync(aggregatedPositions);

            var service = new PowerPositionService(
                mockTradeService.Object,
                mockAggregator.Object,
                mockCsvGenerator.Object
            );

            // Act
            await service.GeneratePowerPositionReportAsync(tradeDate, extractionUtcTimestamp, filePath);

            // Assert
            mockTradeService.Verify(ts => ts.GetTradesAsync(tradeDate), Times.Once);
            mockAggregator.Verify(ta => ta.GetAggregatedPositionAsync(trades), Times.Once);
            mockCsvGenerator.Verify(cg => cg.GenerateCsvReportFile(aggregatedPositions, tradeDate, extractionUtcTimestamp, filePath), Times.Once);
        }
    }
}
