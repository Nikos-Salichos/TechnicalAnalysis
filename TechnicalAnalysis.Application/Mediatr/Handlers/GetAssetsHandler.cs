using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers;

public class GetAssetsHandler : IRequestHandler<GetAssetsQuery, IEnumerable<Asset>>
{
    private readonly IPostgreSqlRepository _repository;

    public GetAssetsHandler(IPostgreSqlRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Asset>> Handle(GetAssetsQuery getAssetsQuery, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAssets();
        if (result.IsError)
        {
            return Enumerable.Empty<Asset>();
        }
        return result.SuccessValue;
    }
}
