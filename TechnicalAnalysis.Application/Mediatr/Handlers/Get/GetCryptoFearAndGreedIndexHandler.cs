using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetCryptoFearAndGreedIndexHandler(IPostgreSqlRepository repository) : IRequestHandler<GetCryptoFearAndGreedIndexQuery, IEnumerable<CryptoFearAndGreedData>>
    {
        public async Task<IEnumerable<CryptoFearAndGreedData>> Handle(GetCryptoFearAndGreedIndexQuery getCryptoFearAndGreedIndexQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetCryptoFearAndGreedIndexAsync();
            if (result.HasError)
            {
                return Enumerable.Empty<CryptoFearAndGreedData>();
            }
            return result.SuccessValue;
        }
    }
}
