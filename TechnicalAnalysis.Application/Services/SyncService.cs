using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Services
{
    public class SyncService : ISyncService
    {
        private readonly ILogger<SyncService> _logger;
        private readonly Func<DataProvider, IAdapter> _adapterFactory;

        public SyncService(ILogger<SyncService> logger, Func<DataProvider, IAdapter> adapterFactory)
        {
            _logger = logger;
            _adapterFactory = adapterFactory;
        }

        public Task<string> SynchronizeProvidersAsync(DataProvider provider, Timeframe timeframe)
        {
            if (timeframe != Timeframe.Daily &&
                (timeframe != Timeframe.Weekly || provider != DataProvider.Binance))
            {
                return Task.FromResult($"Combination {provider} and {timeframe} timeframe is not supported yet");
            }

            return InternalSynchronizeProvidersAsync(provider, timeframe);

            async Task<string> InternalSynchronizeProvidersAsync(DataProvider provider, Timeframe timeframe)
            {
                var adaptersToSync = new List<Task>();

                _logger.LogInformation("Method: {Method} Synchronization started for {Provider}", nameof(SynchronizeProvidersAsync), provider);

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

                if (adaptersToSync.Count > 0)
                {
                    await Task.WhenAll(adaptersToSync);
                }

                return "Synchronization Completed";
            }
        }

        private async Task GetAndSyncAdapter(DataProvider provider, Timeframe timeframe)
        {
            var adapter = _adapterFactory(provider);
            await adapter.Sync(provider, timeframe);
        }
    }
}
