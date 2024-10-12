using TechnicalAnalysis.Domain.Contracts.Input.Binance;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IBinanceClient
    {
        Task<IResult<BinanceExchangeInfoResponse, string>> GetBinanceAssetsAndPairs();
        Task<IResult<object[][], string>> GetBinanceCandlesticks(Dictionary<string, string>? queryParams = null);
    }
}
