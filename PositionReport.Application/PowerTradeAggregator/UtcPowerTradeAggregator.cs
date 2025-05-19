using Microsoft.Extensions.Logging;
using PositionReport.Application.AmbiguousTimeStrategy;
using PositionReport.Application.TimeZoneProvider;
using PositionReport.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.PowerTradeAggregator
{
    public class UtcPowerTradeAggregator : IPowerTradeAggregator
    {
        private readonly ILogger<UtcPowerTradeAggregator> _logger;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly IAmbiguousTimeStrategy _ambiguousTimeStrategy;

        public UtcPowerTradeAggregator(
            ILogger<UtcPowerTradeAggregator> logger,
            ITimeZoneProvider timeZoneProvider,
            IAmbiguousTimeStrategy ambiguousTimeStrategy)
        {
            _logger = logger;
            _timeZoneProvider = timeZoneProvider;
            _ambiguousTimeStrategy = ambiguousTimeStrategy;
        }

        public Task<Dictionary<DateTime, double>> GetAggregatedPositionAsync(IEnumerable<DateTrade> trades)
        {
            Dictionary<DateTime, double> output = new();

            foreach (var trade in trades)
            {
                foreach (var period in trade.Periods)
                {
                    var localDateTime = new DateTime(trade.Date.Year
                        , trade.Date.Month
                        , trade.Date.Day
                        , hour: period.Period - 1 // Periods are 1-24
                        , minute: 0
                        , second: 0
                        , DateTimeKind.Unspecified); // Unspecified to avoid DST issues

                    // Skip invalid times due to DST start
                    if (_timeZoneProvider.GetTimeZone().IsInvalidTime(localDateTime))
                    {
                        _logger.LogWarning("Skipping invalid time {LocalDateTime} for trade date {TradeDate} and period {Period} with time zone {TimeZoneDisplayName}.",
                            localDateTime,
                            trade.Date,
                            period.Period,
                            _timeZoneProvider.GetTimeZone().DisplayName);
                        continue;
                    }

                    // Check for ambiguous times due to DST end
                    // If the time is ambiguous, we can use the IAmbiguousTimeStrategy to resolve it
                    if (_timeZoneProvider.GetTimeZone().IsAmbiguousTime(localDateTime))
                    {
                        foreach (var utcTime in _ambiguousTimeStrategy.ResolveAmbiguousUtcTimes(localDateTime, _timeZoneProvider.GetTimeZone()))
                        {
                            _logger.LogWarning("Ambiguous time {LocalDateTime} mapping to {UtcTime}", localDateTime, utcTime);
                            if (!output.ContainsKey(utcTime))
                                output[utcTime] = 0;

                            output[utcTime] += period.Volume;
                        }
                    }
                    else
                    {
                        // Convert to provided time zone
                        var tradeDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, _timeZoneProvider.GetTimeZone());

                        if (!output.ContainsKey(tradeDateTime))
                            output[tradeDateTime] = 0;

                        output[tradeDateTime] += period.Volume;
                    }      
                }
            }

            return Task.FromResult(output);
        }
    }
}
