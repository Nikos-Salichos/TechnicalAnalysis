using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetAssetsRankingQuery : IRequest<IEnumerable<CoinPaprikaAsset>>
    {
    }
}
