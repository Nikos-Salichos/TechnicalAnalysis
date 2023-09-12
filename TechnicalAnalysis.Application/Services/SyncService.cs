using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using Provider = TechnicalAnalysis.CommonModels.Enums.Provider;

namespace TechnicalAnalysis.Application.Services
{
    public class SyncService : ISyncService
    {
        private readonly ILogger<SyncService> _logger;
        private readonly Func<Provider, IAdapter> _adapterFactory;

        public SyncService(ILogger<SyncService> logger, Func<Provider, IAdapter> adapterFactory)
        {
            _logger = logger;
            _adapterFactory = adapterFactory;
        }

        public Task<string> SynchronizeProvidersAsync(Provider provider, Timeframe timeframe)
        {
            if (timeframe != Timeframe.Daily &&
                (timeframe != Timeframe.Weekly || provider != Provider.Binance))
            {
                return Task.FromResult($"Combination {provider} and {timeframe} timeframe is not supported yet");
            }

            return InternalSynchronizeProvidersAsync(provider, timeframe);

            async Task<string> InternalSynchronizeProvidersAsync(Provider provider, Timeframe timeframe)
            {
                var adaptersToSync = new List<Task>();

                _logger.LogInformation("Method: {Method} Synchronization started for {Provider}", nameof(SynchronizeProvidersAsync), provider);

                if (provider == Provider.Binance || provider == Provider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(Provider.Binance, timeframe));
                }

                if (provider == Provider.Alpaca || provider == Provider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(Provider.Alpaca, timeframe));
                }

                if (provider == Provider.Uniswap || provider == Provider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(Provider.Uniswap, timeframe));
                }

                if (provider == Provider.Pancakeswap || provider == Provider.All)
                {
                    adaptersToSync.Add(GetAndSyncAdapter(Provider.Pancakeswap, timeframe));
                }

                if (adaptersToSync.Count > 0)
                {
                    await Task.WhenAll(adaptersToSync);
                }

                return "Synchronization Completed";
            }
        }

        private Task GetAndSyncAdapter(Provider provider, Timeframe timeframe)
        {
            var adapter = _adapterFactory(provider);
            return adapter.Sync(provider, timeframe);
        }
    }
}
