using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetPoolsHandler(IPostgreSqlRepository repository) : IRequestHandler<GetPoolsQuery, IEnumerable<PoolEntity>>
    {
        public async Task<IEnumerable<PoolEntity>> Handle(GetPoolsQuery getPoolsQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetPoolsAsync();
            if (result.HasError)
            {
                return Enumerable.Empty<PoolEntity>();
            }
            return result.SuccessValue;
        }
    }
}