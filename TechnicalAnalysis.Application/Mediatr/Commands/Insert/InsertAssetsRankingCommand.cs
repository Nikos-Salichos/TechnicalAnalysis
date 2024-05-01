using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertAssetsRankingCommand(List<AssetRanking> assets) : IRequest
    {
        public List<AssetRanking> Assets { get; } = assets;
    }
}
