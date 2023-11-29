using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class PriceFunnel(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool IsFunnel { get; init; }

        public int NumberOfFunnelCandlesticks { get; init; }

        public decimal? HighestPriceOfFunnel { get; init; }

        public long FlagPoleCandlestickId { get; init; }
    }
}
