using Hangfire;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.Domain.Interfaces.Application;

namespace TechnicalAnalysis.Infrastructure.Host.Hangfire
{
    public static class HangfireStartupJob
    {
        public static void EnqueueSynchronizeProvidersJob(WebApplication app)
        {
            var serviceProvider = app.Services;
            var syncService = serviceProvider.GetService<ISyncService>();

            if (syncService != null)
            {
                BackgroundJob.Enqueue(() => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                    CommonModels.Enums.DataProvider.All,
                    CommonModels.Enums.Timeframe.Daily)));
            }
        }
    }
}