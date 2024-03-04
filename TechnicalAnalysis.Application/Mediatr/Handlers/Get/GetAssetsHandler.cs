using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get;

public class GetAssetsHandler(IPostgreSqlRepository repository) : IRequestHandler<GetAssetsQuery, IEnumerable<Asset>>
{
    public async Task<IEnumerable<Asset>> Handle(GetAssetsQuery getAssetsQuery, CancellationToken cancellationToken)
    {
        var result = await repository.GetAssetsAsync();
        if (result.HasError)
        {
            return [];
        }
        return result.SuccessValue;
    }
}
