using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class FlagNestedCandlestickBody(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required bool IsFlag { get; init; }

        public required int NumberOfNestedCandlestickBodies { get; init; }

        public required long FlagPoleCandlestickId { get; init; }
    }
}
