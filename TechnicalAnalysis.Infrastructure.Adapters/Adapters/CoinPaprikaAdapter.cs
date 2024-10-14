using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class CoinPaprikaAdapter(ICoinPaprikaClient coinPaprikaClient, IMediator mediator, ILogger<CoinPaprikaAdapter> logger) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var coinPaprikaProvider = exchanges.Find(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);
            coinPaprikaProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            if (coinPaprikaProvider.IsProviderAssetPairsSyncedToday())
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return true;
            }

            var response = await coinPaprikaClient.SyncAssets();
            if (response.HasError)
            {
                logger.LogError("{DataProvider} response error {FailValue}", nameof(provider), response.FailValue);
                return false;
            }

            var fetchedAssets = await mediator.Send(new GetAssetsRankingQuery());

            var newAssets = new List<AssetRanking>();

            foreach (var asset in response.SuccessValue.Where(c => c.Type is "coin"))
            {
                var pairExists = fetchedAssets.Find(f => string.Equals(f.Name?.Trim(), asset.Name?.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (pairExists is null)
                {
                    newAssets.Add(new AssetRanking
                    {
                        Name = asset.Name,
                        Symbol = asset.Symbol,
                        CreatedDate = (asset.CreatedAt == DateTime.MinValue) ? DateTime.UtcNow : asset.CreatedAt,
                        ProductType = ProductType.Layer1,
                        DataProvider = DataProvider.CoinPaprika,
                    });
                }
            }

            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsRankingCommand(newAssets));
            }

            coinPaprikaProvider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            await mediator.Send(new UpdateExchangeCommand(coinPaprikaProvider.ProviderPairAssetSyncInfo));

            return true;
        }
    }
}
