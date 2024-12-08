using TechnicalAnalysis.Domain.Contracts.Input.Binance;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IBinanceClient
    {
        Task<Result<BinanceExchangeInfoResponse, string>> GetBinanceAssetsAndPairs();
        Task<Result<object[][], string>> GetBinanceCandlesticks(Dictionary<string, string>? queryParams = null);
    }
}
