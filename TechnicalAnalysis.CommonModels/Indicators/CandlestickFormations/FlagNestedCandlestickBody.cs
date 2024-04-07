using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class FlagNestedCandlestickBody(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool IsFlag { get; init; }

        public int NumberOfNestedCandlestickBodies { get; init; }

        public long FlagPoleCandlestickId { get; init; }
    }
}
