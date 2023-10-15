using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetDexCandlesticksHandler : IRequestHandler<GetDexCandlesticksQuery, IEnumerable<DexCandlestick>>
    {
        private readonly IPostgreSqlRepository _repository;

        public GetDexCandlesticksHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DexCandlestick>> Handle(GetDexCandlesticksQuery getDexCandlesticksQuery, CancellationToken cancellationToken)
        {
            var result = await _repository.GetDexCandlestickssAsync();
            if (result.IsError)
            {
                return Enumerable.Empty<DexCandlestick>();
            }
            return result.SuccessValue;
        }
    }
}