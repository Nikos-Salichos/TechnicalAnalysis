using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetPoolsHandler(IPostgreSqlRepository repository) : IRequestHandler<GetPoolsQuery, IEnumerable<Pool>>
    {
        public async Task<IEnumerable<Pool>> Handle(GetPoolsQuery getPoolsQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetPoolsAsync();
            if (result.IsError)
            {
                return Enumerable.Empty<Pool>();
            }
            return result.SuccessValue;
        }
    }
}