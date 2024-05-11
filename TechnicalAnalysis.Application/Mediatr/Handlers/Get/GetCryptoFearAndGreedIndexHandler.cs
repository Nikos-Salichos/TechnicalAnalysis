using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetCryptoFearAndGreedIndexHandler(IPostgreSqlRepository repository) : IRequestHandler<GetCryptoFearAndGreedIndexQuery, List<FearAndGreedModel>>
    {
        public async Task<List<FearAndGreedModel>> Handle(GetCryptoFearAndGreedIndexQuery getCryptoFearAndGreedIndexQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetCryptoFearAndGreedIndexAsync();
            if (result.HasError)
            {
                return [];
            }
            return result.SuccessValue;
        }
    }
}
