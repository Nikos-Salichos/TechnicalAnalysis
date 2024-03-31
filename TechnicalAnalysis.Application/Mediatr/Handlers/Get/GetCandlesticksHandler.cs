using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetCandlesticksHandler(IPostgreSqlRepository repository)
        : IRequestHandler<GetCandlesticksQuery, IEnumerable<CandlestickExtended>>
    {
        public async Task<IEnumerable<CandlestickExtended>> Handle(GetCandlesticksQuery getCoinsQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetCandlesticksAsync();
            if (result.HasError)
            {
                return [];
            }
            return result.SuccessValue.ToDomain();
        }
    }
}
