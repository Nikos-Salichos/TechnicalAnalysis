using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Queries
{
    public class GetPoolsQuery : IRequest<IEnumerable<PoolEntity>>
    {
    }
}
