using Hangfire;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.Domain.Interfaces.Application;

namespace TechnicalAnalysis.Infrastructure.Host.Hangfire
{
    internal static class HangfireStartupJob
    {
        public static async Task EnqueueSynchronizeProvidersJob(WebApplication app)
        {
            var serviceProvider = app.Services;
            var syncService = serviceProvider.GetService<ISyncService>();

            if (syncService != null)
            {
                List<Task> tasks =
                [
                    syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.Alpaca,
                        CommonModels.Enums.Timeframe.Daily)),

                    syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.Binance,
                        CommonModels.Enums.Timeframe.Daily)),

                    syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.AlternativeMeCryptoFearAndGreedProvider,
                        CommonModels.Enums.Timeframe.Daily)),

                   syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.CoinPaprika,
                        CommonModels.Enums.Timeframe.Daily)),

                   syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.CoinMarketCap,
                        CommonModels.Enums.Timeframe.Daily)),

                   syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.CoinRanking,
                        CommonModels.Enums.Timeframe.Daily)),
                ];

                await Task.WhenAll(tasks);

                RecurringJob.AddOrUpdate(
                    "synchronize_providers_job_binance", // Unique identifier for the job
                    () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.Binance,
                        CommonModels.Enums.Timeframe.Daily)),
                   "*/30 * * * *");

                RecurringJob.AddOrUpdate(
                     "synchronize_providers_job_alpaca", // Unique identifier for the job
                     () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                         CommonModels.Enums.DataProvider.Alpaca,
                         CommonModels.Enums.Timeframe.Daily)),
                    "*/30 * * * *");

                RecurringJob.AddOrUpdate("synchronize_providers_job_alternative_me_crypto_and_fear_index",
                    () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.AlternativeMeCryptoFearAndGreedProvider,
                        CommonModels.Enums.Timeframe.Daily)),
                     "*/30 * * * *");

                RecurringJob.AddOrUpdate(
                     "synchronize_providers_job_coinpaprika", // Unique identifier for the job
                     () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                         CommonModels.Enums.DataProvider.CoinPaprika,
                         CommonModels.Enums.Timeframe.Daily)),
                     "0 0 * * *"); // Runs at midnight (00:00) every day

                RecurringJob.AddOrUpdate(
                     "synchronize_providers_job_coinmarketcap", // Unique identifier for the job
                     () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                         CommonModels.Enums.DataProvider.CoinMarketCap,
                         CommonModels.Enums.Timeframe.Daily)),
                     "0 0 * * *"); // Runs at midnight (00:00) every day

                RecurringJob.AddOrUpdate(
                    "synchronize_providers_job_coinranking", // Unique identifier for the job
                    () => syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(
                        CommonModels.Enums.DataProvider.CoinRanking,
                        CommonModels.Enums.Timeframe.Daily)),
                    "0 0 * * *"); // Runs at midnight (00:00) every day
            }
        }
    }
}