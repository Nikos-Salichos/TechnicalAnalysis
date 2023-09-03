using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class FlagNestedCandlestickRange : BaseIndicator
    {
        public bool IsFlag { get; init; }

        public int NumberOfNestedCandlestickRanges { get; init; }

        public long FlagPoleCandlestickId { get; init; }

        public FlagNestedCandlestickRange(long candlestickId)
            : base(candlestickId)
        {
        }
    }
}
