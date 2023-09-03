using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetExchangesHandler : IRequestHandler<GetExchangesQuery, IEnumerable<Exchange>>
    {
        private readonly IPostgreSqlRepository _repository;

        public GetExchangesHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Exchange>> Handle(GetExchangesQuery getProvidersQuery, CancellationToken cancellationToken)
        {
            var providers = await _repository.GetExchanges();
            if (providers.IsError)
            {
                return Enumerable.Empty<Exchange>();
            }
            return providers.SuccessValue;
        }
    }
}
