using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetDexCandlesticksHandler(IPostgreSqlRepository repository)
        : IRequestHandler<GetDexCandlesticksQuery, IEnumerable<DexCandlestick>>
    {
        public async Task<IEnumerable<DexCandlestick>> Handle(GetDexCandlesticksQuery getDexCandlesticksQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetDexCandlesticksAsync();
            if (result.HasError)
            {
                return Enumerable.Empty<DexCandlestick>();
            }
            return result.SuccessValue;
        }
    }
}