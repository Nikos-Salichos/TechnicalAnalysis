using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetExchangesHandler : IRequestHandler<GetPartialProviderQuery, IEnumerable<Provider>>
    {
        private readonly IPostgreSqlRepository _repository;

        public GetExchangesHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Provider>> Handle(GetPartialProviderQuery getProvidersQuery, CancellationToken cancellationToken)
        {
            var providers = await _repository.GetProviders();
            if (providers.IsError)
            {
                return Enumerable.Empty<Provider>();
            }
            return providers.SuccessValue;
        }
    }
}
