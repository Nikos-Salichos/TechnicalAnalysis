using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetCoinPaprikaAssetsHandler(IPostgreSqlRepository repository) : IRequestHandler<GetCoinPaprikaAssetsQuery, IEnumerable<CoinPaprikaAsset>>
    {
        public async Task<IEnumerable<CoinPaprikaAsset>> Handle(GetCoinPaprikaAssetsQuery getCoinPaprikaAssetsQuery, CancellationToken cancellationToken)
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