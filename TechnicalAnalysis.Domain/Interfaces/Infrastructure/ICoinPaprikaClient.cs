using TechnicalAnalysis.Domain.Contracts.Input.CoinPaprika;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICoinPaprikaClient
    {
        Task<IResult<List<CoinPaprikaAssetContract>, string>> SyncAssets();
    }
}
