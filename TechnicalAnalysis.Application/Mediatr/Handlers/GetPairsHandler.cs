using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetPairsHandler : IRequestHandler<GetPairsQuery, IEnumerable<PairExtended>>
    {
        private readonly IPostgreSqlRepository _repository;

        public GetPairsHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PairExtended>> Handle(GetPairsQuery getCoinsQuery, CancellationToken cancellationToken)
        {
            var result = await _repository.GetPairs();
            if (result.IsError)
            {
                return Enumerable.Empty<PairExtended>();
            }
            return result.SuccessValue.ToDomain();
        }
    }
}