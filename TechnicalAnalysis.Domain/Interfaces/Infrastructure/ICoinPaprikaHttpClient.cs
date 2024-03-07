using TechnicalAnalysis.Domain.Contracts.Input.CoinPaprika;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICoinPaprikaHttpClient
    {
        Task<IResult<IEnumerable<CoinPaprikaAssetContract>, string>> SyncAssets();
    }
}
