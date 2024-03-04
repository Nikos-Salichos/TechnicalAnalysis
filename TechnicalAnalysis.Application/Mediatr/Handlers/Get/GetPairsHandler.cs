using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetPairsHandler(IPostgreSqlRepository repository) : IRequestHandler<GetPairsQuery, IEnumerable<PairExtended>>
    {
        public async Task<IEnumerable<PairExtended>> Handle(GetPairsQuery getCoinsQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetPairsAsync();
            if (result.HasError)
            {
                return Enumerable.Empty<PairExtended>();
            }
            return result.SuccessValue.ToDomain();
        }
    }
}