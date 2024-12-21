using TechnicalAnalysis.Domain.Contracts.Input.CoinMarketCap;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICoinMarketCapClient
    {
        Task<Result<CoinMarketCapAssetContract, string>> SyncAssets();
    }
}
