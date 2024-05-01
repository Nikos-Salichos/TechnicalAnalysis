using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class VerticalHorizontalFilter(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required double Value { get; init; }
    }
}
