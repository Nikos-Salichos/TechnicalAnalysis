using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoAndFearIndex;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetCryptoFearAndGreedIndexHandler(IPostgreSqlRepository repository) : IRequestHandler<GetCryptoFearAndGreedIndexQuery, IEnumerable<CryptoFearAndGreedData>>
    {
        public async Task<IEnumerable<CryptoFearAndGreedData>> Handle(GetCryptoFearAndGreedIndexQuery getCryptoFearAndGreedIndexQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetCryptoFearAndGreedIndexAsync();
            if (result.IsError)
            {
                return Enumerable.Empty<CryptoFearAndGreedData>();
            }
            return result.SuccessValue;
        }
    }
}
