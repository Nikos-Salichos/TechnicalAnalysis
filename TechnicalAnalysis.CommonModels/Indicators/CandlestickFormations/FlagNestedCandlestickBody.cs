using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class FlagNestedCandlestickBody : BaseIndicator
    {
        public bool IsFlag { get; init; }

        public int NumberOfNestedCandlestickBodies { get; init; }

        public long FlagPoleCandlestickId { get; init; }

        public FlagNestedCandlestickBody(long candlestickId)
            : base(candlestickId)
        {
        }

    }
}
