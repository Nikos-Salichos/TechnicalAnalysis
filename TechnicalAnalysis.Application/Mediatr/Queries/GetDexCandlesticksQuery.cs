using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Queries
{
    public class GetDexCandlesticksQuery : IRequest<IEnumerable<DexCandlestick>>
    {
    }
}
