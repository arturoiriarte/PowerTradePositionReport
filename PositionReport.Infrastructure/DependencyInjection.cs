using Microsoft.Extensions.DependencyInjection;
using PositionReport.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IPowerTradeService, AxpoPowerTradeServiceAdapter>();
            services.AddTransient<IPowerPositionCsvGenerator, PowerPositionSimpleCsvGenerator>();
            return services;
        }
    }
}
