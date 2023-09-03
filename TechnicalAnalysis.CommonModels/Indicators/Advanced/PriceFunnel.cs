using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class PriceFunnel : BaseIndicator
    {
        public bool IsFunnel { get; init; }

        public int NumberOfFunnelCandlesticks { get; init; }

        public decimal? HighestPriceOfFunnel { get; init; }

        public long FlagPoleCandlestickId { get; init; }

        public PriceFunnel(long candlestickId) : base(candlestickId)
        {
        }

    }
}
