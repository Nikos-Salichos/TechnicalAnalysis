using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.JsonOutput;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromDomainToOutputContract
    {
        private static Candle ToOutputContract(this CandlestickExtended domainCandlestick)
        {
            return new Candle
            {
                OpenedAt = domainCandlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Open = domainCandlestick.OpenPrice,
                High = domainCandlestick.HighPrice,
                Low = domainCandlestick.LowPrice,
                Close = domainCandlestick.ClosePrice,
                EnhancedScans = domainCandlestick.EnhancedScans
            };
        }

        private static IEnumerable<Candle> ToOutputContract(this IEnumerable<CandlestickExtended> domainCandlesticks)
        {
            return domainCandlesticks is null
                ? Enumerable.Empty<Candle>()
                : domainCandlesticks.Select(c => c.ToOutputContract());
        }

        public static PartialPair ToOutputContract(this PairExtended domain)
        {
            return new PartialPair
            {
                Symbol = domain.Symbol,
                Candles = domain.Candlesticks.ToOutputContract()
            };
        }
    }
}
