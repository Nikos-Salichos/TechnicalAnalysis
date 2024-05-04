using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Services
{
    public class SyncService(ILogger<SyncService> logger, Func<DataProvider, IAdapter> adapterFactory, IMediator mediator) : ISyncService
    {
        public async Task SynchronizeProvidersAsync(DataProviderTimeframeRequest dataProviderTimeframeRequest)
        {
            var exchanges = await mediator.Send(new GetProviderSynchronizationQuery());

            await InternalSynchronizeProvidersAsync(dataProviderTimeframeRequest.DataProvider, dataProviderTimeframeRequest.Timeframe, exchanges);

            async Task InternalSynchronizeProvidersAsync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
            {
                var adaptersToSync = new List<Task>();

                logger.LogInformation("Synchronization started for {Provider}", provider);

                if (provider == DataProvider.Binance || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Binance, timeframe, exchanges));
                }

                if (provider == DataProvider.Alpaca || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Alpaca, timeframe, exchanges));
                }

                if (provider == DataProvider.Uniswap || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Uniswap, timeframe, exchanges));
                }

                if (provider == DataProvider.Pancakeswap || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Pancakeswap, timeframe, exchanges));
                }

                if (provider == DataProvider.WallStreetZen || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.WallStreetZen, timeframe, exchanges));
                }

                if (provider == DataProvider.AlternativeMeCryptoFearAndGreedProvider || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.AlternativeMeCryptoFearAndGreedProvider, timeframe, exchanges));
                }

                if (provider == DataProvider.CoinPaprika || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.CoinPaprika, timeframe, exchanges));
                }

                if (provider == DataProvider.CoinMarketCap || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.CoinMarketCap, timeframe, exchanges));
                }

                if (provider == DataProvider.CoinRanking || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.CoinRanking, timeframe, exchanges));
                }

                if (provider == DataProvider.RapidApiStockFearAndGreedProvider || provider == DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.RapidApiStockFearAndGreedProvider, timeframe, exchanges));
                }

                if (adaptersToSync.Count > 0)
                {
                    await Task.WhenAll(adaptersToSync);
                }
            }
        }

        private async Task GetAndSyncAdapter(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var adapter = adapterFactory(provider);
            var providerSynced = await adapter.Sync(provider, timeframe, exchanges);
            if (providerSynced)
            {
                logger.LogInformation("Synchronization completed for {Provider}", provider);
            }
            else
            {
                logger.LogError("Synchronization failed for {Provider}", provider);
            }
        }
    }
}
