using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Queries
{
    public class GetPairsQuery : IRequest<IEnumerable<PairExtended>>
    {
    }
}
