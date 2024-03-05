using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertAssetsRankingCommand(IEnumerable<CoinPaprikaAsset> assets) : IRequest
    {
        public IEnumerable<CoinPaprikaAsset> Assets { get; } = assets;
    }
}
