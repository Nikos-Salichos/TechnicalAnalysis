using TechnicalAnalysis.Domain.Contracts.Input.CoinMarketCap;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICoinMarketCapHttpClient
    {
        Task<IResult<CoinMarketCapAssetContract, string>> SyncAssets();
    }
}
