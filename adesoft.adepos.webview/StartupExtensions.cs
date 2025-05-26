using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Radzen;

namespace adesoft.adepos.webview
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IPlantService, PlantService>();
            services.AddTransient<ILogisticMasterDataService, LogisticMasterDataService>();
            services.AddTransient<ILogisticsCoreService, LogisticsCoreService>();
            services.AddTransient<ILedgerBalanceService, LedgerBalanceService>();
            services.AddTransient<IElectronicBillingService, ElectronicBillingService>();
            services.AddTransient<ISqlManagerService, SqlManagerService>();

            return services;
        }
    }
}
