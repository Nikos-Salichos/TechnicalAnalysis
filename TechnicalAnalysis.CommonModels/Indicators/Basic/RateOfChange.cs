using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class RateOfChange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required double? Value { get; init; }
    }
}
