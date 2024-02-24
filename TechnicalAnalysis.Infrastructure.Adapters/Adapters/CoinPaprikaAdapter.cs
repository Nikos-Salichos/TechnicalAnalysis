using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class CoinPaprikaAdapter(ICoinPaprikaClient coinPaprikaClient, IMediator mediator, ILogger<CoinPaprikaAdapter> logger) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe)
        {
            var response = await coinPaprikaClient.SyncAssets();

            if (response.HasError)
            {
                return false;
            }

            //Fetch old assets
            var fetchedAssets = await mediator.Send(new GetAssetsQuery());

            //Find differential;
            var newAssets = response.SuccessValue.ToDomain().Except(fetchedAssets).Distinct().ToList();

            //Insert differential
            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsCommand(newAssets));
            }

            return true;
        }
    }
}
