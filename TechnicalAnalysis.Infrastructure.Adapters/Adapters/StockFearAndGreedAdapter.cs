using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class StockFearAndGreedAdapter(IStockFearAndGreedHttpClient stockFearAndGreedHttpClient, IMediator mediator) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var stockFearAndGreedIndexData = await mediator.Send(new GetStockFearAndGreedIndexQuery());

            var latestDatetime = stockFearAndGreedIndexData.FirstOrDefault()?.DateTime ?? DateTime.MinValue;

            if (latestDatetime <= DateTime.UtcNow.Date)
            {
                var response = await stockFearAndGreedHttpClient.GetStockFearAndGreedIndex();
                if (response.HasError)
                {
                    return false;
                }

                var stockFearAndGreedDomain = response.SuccessValue.ToDomain();
                await mediator.Send(new InsertStockFearAndGreedIndexCommand(stockFearAndGreedDomain));
            }

            return true;
        }
    }
}
