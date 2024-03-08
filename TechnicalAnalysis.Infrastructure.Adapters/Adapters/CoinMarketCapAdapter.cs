using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    internal class CoinMarketCapAdapter(ICoinMarketCapHttpClient coinMarketCapHttpClient, IMediator mediator, ILogger<CoinMarketCapAdapter> logger)
        : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe)
        {
            var response = await coinMarketCapHttpClient.SyncAssets();
            if (response.HasError)
            {
                logger.LogError("CoinPaprika response error {FailValue}", response.FailValue);
                return false;
            }

            var fetchedAssets = (await mediator.Send(new GetAssetsRankingQuery())).ToList();

            var newAssets = new List<AssetRanking>();
            foreach (var data in response.SuccessValue.Data)
            {
                var pairExists = fetchedAssets.Find(f => string.Equals(f.Name, data.Name, StringComparison.InvariantCultureIgnoreCase));
                if (pairExists is not null)
                {
                    newAssets.Add(new AssetRanking
                    {
                        Name = data.Name,
                        Symbol = data.Symbol,
                        CreatedDate = data.DateAdded,
                        AssetType = AssetType.Layer1,
                        DataProvider = DataProvider.CoinMarketCap
                    });
                }
            }

            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsRankingCommand(newAssets));
            }

            return true;
        }
    }
}
