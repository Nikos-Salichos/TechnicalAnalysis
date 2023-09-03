using TechnicalAnalysis.Domain.Contracts.Input.Binance;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IBinanceHttpClient
    {
        Task<IResult<BinanceExchangeInfoResponse, string>> GetBinanceAssetsAndPairs();
        Task<IResult<object[][], string>> GetBinanceCandlesticks(IDictionary<string, string>? queryParams = null);
    }
}
