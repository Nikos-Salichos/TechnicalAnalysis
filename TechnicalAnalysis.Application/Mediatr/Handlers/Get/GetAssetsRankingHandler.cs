using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetAssetsRankingHandler(IPostgreSqlRepository repository) : IRequestHandler<GetAssetsRankingQuery, IEnumerable<CoinPaprikaAsset>>
    {
        public async Task<IEnumerable<CoinPaprikaAsset>> Handle(GetAssetsRankingQuery getCoinPaprikaAssetsQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetCoinPaprikaAssetsAsync();
            if (result.HasError)
            {
                return [];
            }
            return result.SuccessValue;
        }
    }
}