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

        public async Task SynchronizeProvidersAsync()
        {
            var wallStreetZenAdapter = _adapterFactory(Provider.WallStreetZen);
            //await wallStreetZenAdapter.Sync(Provider.WallStreetZen);

            _logger.LogInformation("Synchronization started for {Alpaca}, {Binance}, {Uniswap}, {Pancakeswap}",
                nameof(Provider.Alpaca), nameof(Provider.Binance), nameof(Provider.Uniswap), nameof(Provider.Pancakeswap));

            var binanceAdapter = _adapterFactory(Provider.Binance);
            var alpacaAdapter = _adapterFactory(Provider.Alpaca);
            var uniswapAdapter = _adapterFactory(Provider.Uniswap);
            var pancakeswapAdapter = _adapterFactory(Provider.Pancakeswap);

            await Task.WhenAll(alpacaAdapter.Sync(Provider.Alpaca),
                   binanceAdapter.Sync(Provider.Binance),
                 uniswapAdapter.Sync(Provider.Uniswap),
                 pancakeswapAdapter.Sync(Provider.Pancakeswap));
        }
    }
}
