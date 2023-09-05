using Microsoft.Extensions.Logging;
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

        public async Task SynchronizeProvidersAsync(Provider provider)
        {
            var adaptersToSync = new List<Task>();

            if (provider == Provider.Binance || provider == Provider.All)
            {
                _logger.LogInformation("Method: {Method} Synchronization started for {Provider}", nameof(SynchronizeProvidersAsync), provider);
                adaptersToSync.Add(GetAndSyncAdapter(Provider.Binance));
            }

            if (provider == Provider.Alpaca || provider == Provider.All)
            {
                _logger.LogInformation("Method: {Method} Synchronization started for {Provider}", nameof(SynchronizeProvidersAsync), provider);
                adaptersToSync.Add(GetAndSyncAdapter(Provider.Alpaca));
            }

            if (provider == Provider.Uniswap || provider == Provider.All)
            {
                _logger.LogInformation("Method: {Method} Synchronization started for {Provider}", nameof(SynchronizeProvidersAsync), provider);
                adaptersToSync.Add(GetAndSyncAdapter(Provider.Uniswap));
            }

            if (provider == Provider.Pancakeswap || provider == Provider.All)
            {
                _logger.LogInformation("Method: {Method} Synchronization started for {Provider}", nameof(SynchronizeProvidersAsync), provider);
                adaptersToSync.Add(GetAndSyncAdapter(Provider.Pancakeswap));
            }

            if (adaptersToSync.Count > 0)
            {
                await Task.WhenAll(adaptersToSync);
            }
        }

        private Task GetAndSyncAdapter(Provider provider)
        {
            var adapter = _adapterFactory(provider);
            return adapter.Sync(provider);
        }
    }
}
