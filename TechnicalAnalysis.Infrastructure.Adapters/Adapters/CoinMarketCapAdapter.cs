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
    internal sealed class CoinMarketCapAdapter(ICoinMarketCapHttpClient coinMarketCapHttpClient, IMediator mediator, ILogger<CoinMarketCapAdapter> logger)
        : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var coinMarketCapProvider = exchanges.Find(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);
            coinMarketCapProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            if (coinMarketCapProvider.IsProviderAssetPairsSyncedToday())
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return true;
            }

            var response = await coinMarketCapHttpClient.SyncAssets();
            if (response.HasError)
            {
                logger.LogError("{DataProvider} response error {FailValue}", nameof(provider), response.FailValue);
                return false;
            }

            var fetchedAssets = await mediator.Send(new GetAssetsRankingQuery());

            var newAssets = new List<AssetRanking>();
            foreach (var data in response.SuccessValue.Data)
            {
                var pairExists = fetchedAssets.Find(f => string.Equals(f.Name.Trim(), data.Name.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (pairExists is null)
                {
                    newAssets.Add(new AssetRanking
                    {
                        Name = data.Name,
                        Symbol = data.Symbol,
                        CreatedDate = data.DateAdded,
                        AssetType = ProductType.Layer1,
                        DataProvider = DataProvider.CoinMarketCap
                    });
                }
            }

            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsRankingCommand(newAssets));
            }

            coinMarketCapProvider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            await mediator.Send(new UpdateExchangeCommand(coinMarketCapProvider.ProviderPairAssetSyncInfo));

            return true;
        }
    }
}
