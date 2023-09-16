using Hangfire;
using TechnicalAnalysis.CommonModels.ApiRequests;
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
                var dataProviderTimeframeRequest = new DataProviderTimeframeRequest();
                BackgroundJob.Enqueue(() => syncService.SynchronizeProvidersAsync(dataProviderTimeframeRequest));
            }
        }
    }
}
