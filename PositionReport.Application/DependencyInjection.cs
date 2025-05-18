using Microsoft.Extensions.DependencyInjection;
using PositionReport.Application.AmbiguousTimeStrategy;
using PositionReport.Application.PowerPositionRunner;
using PositionReport.Application.PowerPositionScheduler;
using PositionReport.Application.PowerTradeAggregator;
using PositionReport.Application.TimeZoneProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IPowerPositionScheduler, PowerPositionSimpleSchedulerRunImmediately>();
            services.AddSingleton<ITimeZoneProvider, BerlinTimeZoneProvider>();
            services.AddSingleton<IAmbiguousTimeStrategy, FirstUtcAmbiguousTimeStrategy>();
            services.AddSingleton<IPowerPositionRunnerWithRetry, PowerPositionRunnerWithMaxRetry>();
            services.AddTransient<IPowerTradeAggregator, UtcPowerTradeAggregator>();
            services.AddTransient<IPowerPositionService, PowerPositionService>();
            return services;
        }
    }
}
