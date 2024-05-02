using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetPoolsHandler(IPostgreSqlRepository repository) : IRequestHandler<GetPoolsQuery, List<PoolEntity>>
    {
        public async Task<List<PoolEntity>> Handle(GetPoolsQuery getPoolsQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetPoolsAsync();
            if (result.HasError)
            {
                return [];
            }
            return result.SuccessValue;
        }
    }
}