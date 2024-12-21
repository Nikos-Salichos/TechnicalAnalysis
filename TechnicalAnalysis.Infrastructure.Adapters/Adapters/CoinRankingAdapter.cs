using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.CoinRanking;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public sealed class CoinRankingAdapter(ICoinRankingClient coinRankingHttpClient, IMediator mediator, ILogger<CoinRankingAdapter> logger)
    : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var coinRankingProvider = exchanges.Find(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);
            coinRankingProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            if (coinRankingProvider.IsProviderAssetPairsSyncedToday())
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return true;
            }

            var fetchedAssets = await mediator.Send(new GetAssetsRankingQuery());

            const int initialPage = 0;
            var initialResponse = await coinRankingHttpClient.SyncAssets(initialPage);
            if (initialResponse.HasError)
            {
                logger.LogError("{DataProvider} initial response error {FailValue}", nameof(provider), initialResponse.FailValue);
                return false;
            }

            await ProcessNewAssets(fetchedAssets, initialResponse.SuccessValue.Data.Coins);

            coinRankingProvider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            await mediator.Send(new UpdateExchangeCommand(coinRankingProvider.ProviderPairAssetSyncInfo));

            return true;
        }

        private async Task ProcessNewAssets(List<AssetRanking> existingAssets, List<Coin> newCoins)
        {
            var newAssets = new List<AssetRanking>();
            foreach (var newCoin in newCoins)
            {
                var pairExists = existingAssets.Find(f => string.Equals(f.Name?.Trim(), newCoin.Name.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (pairExists is null)
                {
                    newAssets.Add(new AssetRanking
                    {
                        Name = newCoin.Name,
                        Symbol = newCoin.Symbol,
                        CreatedDate = DateTime.UnixEpoch.AddSeconds(newCoin.ListedAt),
                        ProductType = ProductType.Layer1,
                        DataProvider = DataProvider.CoinMarketCap
                    });
                }
            }

            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsRankingCommand(newAssets));
            }
        }
    }
}
