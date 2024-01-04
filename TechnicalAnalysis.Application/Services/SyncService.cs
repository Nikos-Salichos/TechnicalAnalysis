using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Services
{
    public class SyncService(ILogger<SyncService> logger, Func<DataProvider, IAdapter> adapterFactory) : ISyncService
    {
        public Task SynchronizeProvidersAsync(DataProviderTimeframeRequest dataProviderTimeframeRequest)
        {
            return InternalSynchronizeProvidersAsync(dataProviderTimeframeRequest.DataProvider, dataProviderTimeframeRequest.Timeframe);

            async Task InternalSynchronizeProvidersAsync(DataProvider provider, Timeframe timeframe)
            {
                var adaptersToSync = new List<Task>();

                logger.LogInformation("Synchronization started for {Provider}", provider);

                if (provider == DataProvider.Binance || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Binance, timeframe));
                }

                if (provider == DataProvider.Alpaca || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Alpaca, timeframe));
                }

                if (provider == DataProvider.Uniswap || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Uniswap, timeframe));
                }

                if (provider == DataProvider.Pancakeswap || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Pancakeswap, timeframe));
                }

                if (provider == DataProvider.WallStreetZen || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.WallStreetZen, timeframe));
                }

                if (provider == DataProvider.AlternativeMeCryptoAndFearIndex || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.AlternativeMeCryptoAndFearIndex, timeframe));
                }

                if (adaptersToSync.Count > 0)
                {
                    await Task.WhenAll(adaptersToSync);
                }
            }
        }

        private async Task GetAndSyncAdapter(DataProvider provider, Timeframe timeframe)
        {
            var adapter = adapterFactory(provider);
            await adapter.Sync(provider, timeframe);
        }
    }
}
