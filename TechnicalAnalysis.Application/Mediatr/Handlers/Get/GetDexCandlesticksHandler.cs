using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetDexCandlesticksHandler(IPostgreSqlRepository repository) : IRequestHandler<GetDexCandlesticksQuery, List<DexCandlestick>>
    {
        public async Task<List<DexCandlestick>> Handle(GetDexCandlesticksQuery getDexCandlesticksQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetDexCandlesticksAsync();
            if (result.HasError)
            {
                return [];
            }
            return result.SuccessValue;
        }
    }
}