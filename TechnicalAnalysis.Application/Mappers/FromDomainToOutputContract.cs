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

        public static IEnumerable<Candle> ToOutputContract(this IEnumerable<CandlestickExtended> domainCandlesticks)
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

        public static IEnumerable<PartialPair> ToOutputContract(this IEnumerable<PairExtended> pairExtendeds)
        {
            return pairExtendeds is null
                ? Enumerable.Empty<PartialPair>()
                : pairExtendeds.Select(c => c.ToOutputContract());
        }
    }
}
