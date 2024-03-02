using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertCoinPaprikaAssetCommand(IEnumerable<CoinPaprikaAsset> assets) : IRequest
    {
        public IEnumerable<CoinPaprikaAsset> Assets { get; } = assets;
    }
}
