using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class GetCoinPaprikaAssetsQuery : IRequest<IEnumerable<CoinPaprikaAsset>>
    {
    }
}
