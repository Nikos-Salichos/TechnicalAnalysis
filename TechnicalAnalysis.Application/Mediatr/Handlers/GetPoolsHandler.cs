using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetPoolsHandler : IRequestHandler<GetPoolsQuery, IEnumerable<Pool>>
    {
        private readonly IPostgreSqlRepository _repository;

        public GetPoolsHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Pool>> Handle(GetPoolsQuery getPoolsQuery, CancellationToken cancellationToken)
        {
            var result = await _repository.GetPoolsAsync();
            if (result.IsError)
            {
                return Enumerable.Empty<Pool>();
            }
            return result.SuccessValue;
        }
    }
}