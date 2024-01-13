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
                RecurringJob.AddOrUpdate(
                    "synchronize_providers_job_binance", // Unique identifier for the job
                    () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.Binance,
                        CommonModels.Enums.Timeframe.Daily)),
                   "*/30 * * * *"); // Run at the current minute and then every 30 minutes

                RecurringJob.AddOrUpdate(
                    "synchronize_providers_job_alpaca", // Unique identifier for the job
                    () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.Alpaca,
                        CommonModels.Enums.Timeframe.Daily)),
                   "*/30 * * * *"); // Run at the current minute and then every 30 minutes

                RecurringJob.AddOrUpdate("synchronize_providers_job_alternative_me_crypto_and_fear_index",
                    () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.AlternativeMeCryptoAndFearIndex,
                        CommonModels.Enums.Timeframe.Daily)),
                     "*/30 * * * *"); // Run at the current minute and then every 30 minutes
            }
        }
    }
}