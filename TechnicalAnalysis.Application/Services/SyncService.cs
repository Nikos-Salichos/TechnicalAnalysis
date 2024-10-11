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

                if (provider is DataProvider.Binance or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Binance, timeframe, exchanges));
                }

                if (provider is DataProvider.Alpaca or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Alpaca, timeframe, exchanges));
                }

                if (provider is DataProvider.Uniswap or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Uniswap, timeframe, exchanges));
                }

                if (provider is DataProvider.Pancakeswap or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.Pancakeswap, timeframe, exchanges));
                }

                if (provider is DataProvider.WallStreetZen or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.WallStreetZen, timeframe, exchanges));
                }

                if (provider is DataProvider.AlternativeMeCryptoFearAndGreedProvider or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.AlternativeMeCryptoFearAndGreedProvider, timeframe, exchanges));
                }

                if (provider is DataProvider.CoinPaprika or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.CoinPaprika, timeframe, exchanges));
                }

                if (provider is DataProvider.CoinMarketCap or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.CoinMarketCap, timeframe, exchanges));
                }

                if (provider is DataProvider.CoinRanking or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.CoinRanking, timeframe, exchanges));
                }

                if (provider is DataProvider.RapidApiStockFearAndGreedProvider or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.RapidApiStockFearAndGreedProvider, timeframe, exchanges));
                }

                if (provider is DataProvider.CnnApiStockFearAndGreedProvider or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.CnnApiStockFearAndGreedProvider, timeframe, exchanges));
                }

                if (provider is DataProvider.FredApiProvider or DataProvider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(DataProvider.FredApiProvider, timeframe, exchanges));
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
                if (provider == DataProvider.CnnApiStockFearAndGreedProvider)
                {
                    logger.LogError("Synchronization failed for {CnnApiStockFearAndGreedProvider} and we trigger the next {RapidApiStockFearAndGreedProvider}",
                        DataProvider.CnnApiStockFearAndGreedProvider, DataProvider.RapidApiStockFearAndGreedProvider);

                    providerSynced = await adapter.Sync(DataProvider.RapidApiStockFearAndGreedProvider, timeframe, exchanges);
                    provider = DataProvider.RapidApiStockFearAndGreedProvider;
                }

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
}
