using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertAssetsRankingCommand(IEnumerable<AssetRanking> assets) : IRequest
    {
        public IEnumerable<AssetRanking> Assets { get; } = assets;
    }
}
