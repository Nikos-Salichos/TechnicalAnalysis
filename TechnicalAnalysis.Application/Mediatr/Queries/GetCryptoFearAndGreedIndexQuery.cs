using MediatR;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;

namespace TechnicalAnalysis.Application.Mediatr.Queries
{
    public class GetCryptoFearAndGreedIndexQuery : IRequest<IEnumerable<CryptoFearAndGreedData>>
    {
    }
}