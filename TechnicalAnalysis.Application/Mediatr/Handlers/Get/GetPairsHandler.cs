using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetPairsHandler(IPostgreSqlRepository repository) : IRequestHandler<GetPairsQuery, List<PairExtended>>
    {
        public async Task<List<PairExtended>> Handle(GetPairsQuery getCoinsQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetPairsAsync();
            if (result.HasError)
            {
                return [];
            }
            return result.SuccessValue.ToDomain();
        }
    }
}