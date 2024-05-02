using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetStockFearAndGreedIndexHandler(IPostgreSqlRepository repository) : IRequestHandler<GetStockFearAndGreedIndexQuery, List<StockFearAndGreedDomain>>
    {
        public async Task<List<StockFearAndGreedDomain>> Handle(GetStockFearAndGreedIndexQuery getCryptoFearAndGreedIndexQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetStockFearAndGreedIndexAsync();
            if (result.HasError)
            {
                return [];
            }
            return result.SuccessValue;
        }
    }
}
