using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetExchangesHandler : IRequestHandler<GetProviderSynchronizationQuery, IEnumerable<ProviderSynchronization>>
    {
        private readonly IPostgreSqlRepository _repository;

        public GetExchangesHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProviderSynchronization>> Handle(GetProviderSynchronizationQuery getProvidersQuery, CancellationToken cancellationToken)
        {
            var providers = await _repository.GetProviders();
            if (providers.IsError)
            {
                return Enumerable.Empty<ProviderSynchronization>();
            }
            return providers.SuccessValue;
        }
    }
}
