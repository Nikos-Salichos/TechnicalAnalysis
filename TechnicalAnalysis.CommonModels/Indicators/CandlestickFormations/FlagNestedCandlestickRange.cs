using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class FlagNestedCandlestickRange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool IsFlag { get; init; }

        public int NumberOfNestedCandlestickRanges { get; init; }

        public long FlagPoleCandlestickId { get; init; }
    }
}
