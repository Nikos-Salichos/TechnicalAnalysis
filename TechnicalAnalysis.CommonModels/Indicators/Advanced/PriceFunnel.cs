using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class PriceFunnel(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required bool IsFunnel { get; init; }

        public required int NumberOfFunnelCandlesticks { get; init; }

        public decimal? HighestPriceOfFunnel { get; init; }

        public long FlagPoleCandlestickId { get; init; }
    }
}
