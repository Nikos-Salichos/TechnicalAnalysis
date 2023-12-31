using MediatR;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoAndFearIndex;

namespace TechnicalAnalysis.Application.Mediatr.Queries
{
    public class GetCryptoFearAndGreedIndexQuery : IRequest<IEnumerable<CryptoFearAndGreedData>>
    {
    }
}