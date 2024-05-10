using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.OutputContract;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromDomainToOutputContract
    {
        private static Candle ToOutputContract(this CandlestickExtended domainCandlestick)
            => new()
            {
                OpenedAt = domainCandlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Open = domainCandlestick.OpenPrice,
                High = domainCandlestick.HighPrice,
                Low = domainCandlestick.LowPrice,
                Close = domainCandlestick.ClosePrice,
                EnhancedScans = domainCandlestick.EnhancedScans
            };

        private static List<Candle> ToOutputContract(this List<CandlestickExtended> domainCandlesticks)
            => domainCandlesticks.ConvertAll(c => c.ToOutputContract());

        public static PartialPair ToOutputContract(this PairExtended domain)
            => new()
            {
                Symbol = domain.Symbol,
                Candles = domain.Candlesticks.ToOutputContract()
            };
    }
}
