using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetExchangesHandler(IPostgreSqlRepository repository)
        : IRequestHandler<GetProviderSynchronizationQuery, IEnumerable<ProviderSynchronization>>
    {
        public async Task<IEnumerable<ProviderSynchronization>> Handle(GetProviderSynchronizationQuery getProvidersQuery, CancellationToken cancellationToken)
        {
            var providers = await repository.GetProvidersAsync();
            if (providers.IsError)
            {
                return Enumerable.Empty<ProviderSynchronization>();
            }
            return providers.SuccessValue;
        }
    }
}
