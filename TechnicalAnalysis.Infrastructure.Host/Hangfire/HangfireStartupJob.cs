using Hangfire;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;

namespace TechnicalAnalysis.Infrastructure.Host.Hangfire
{
    public class HangfireStartupJob
    {
        public static void EnqueueSynchronizeProvidersJob(WebApplication app)
        {
            var serviceProvider = app.Services;
            var syncService = serviceProvider.GetService<ISyncService>();

            if (syncService != null)
            {
                BackgroundJob.Enqueue(() => syncService.SynchronizeProvidersAsync(DataProvider.Alpaca, Timeframe.Daily));
            }
        }
    }
}
