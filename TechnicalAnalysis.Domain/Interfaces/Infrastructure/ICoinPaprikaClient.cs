using TechnicalAnalysis.Domain.Contracts.Input.CoinPaprika;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICoinPaprikaClient
    {
        Task<Result<List<CoinPaprikaAssetContract>, string>> SyncAssets();
    }
}
