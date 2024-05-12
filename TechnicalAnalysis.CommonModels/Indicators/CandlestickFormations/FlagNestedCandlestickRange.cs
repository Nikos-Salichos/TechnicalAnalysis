using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class FlagNestedCandlestickRange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required bool IsFlag { get; init; }

        public required int NumberOfNestedCandlestickRanges { get; init; }

        public required long FlagPoleCandlestickId { get; init; }
    }
}
