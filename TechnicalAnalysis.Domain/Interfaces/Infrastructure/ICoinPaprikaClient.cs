using TechnicalAnalysis.Domain.Contracts.Output.CoinPaprika;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICoinPaprikaClient
    {
        Task<IResult<IEnumerable<CoinPaprikaAsset>, string>> SyncAssets();
    }
}
