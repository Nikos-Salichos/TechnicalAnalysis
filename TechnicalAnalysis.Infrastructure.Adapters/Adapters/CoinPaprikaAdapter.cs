using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class CoinPaprikaAdapter(ICoinPaprikaHttpClient coinPaprikaClient, IMediator mediator, ILogger<CoinPaprikaAdapter> logger) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe)
        {
            var response = await coinPaprikaClient.SyncAssets();
            if (response.HasError)
            {
                logger.LogError("CoinPaprika response error {FailValue}", response.FailValue);
                return false;
            }

            var newLayerOneAssets = response.SuccessValue.Where(c => c.Type is "coin").ToList();

            var fetchedAssets = await mediator.Send(new GetAssetsRankingQuery());

            var newAssets = newLayerOneAssets.ToDomain().Except(fetchedAssets).Distinct().ToList();
            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsRankingCommand(newAssets));
            }

            return true;
        }
    }
}
