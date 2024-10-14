using TechnicalAnalysis.Domain.Contracts.Input.CoinRanking;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICoinRankingClient
    {
        Task<IResult<CoinRankingAssetContract, string>> SyncAssets(int offset);
    }
}
