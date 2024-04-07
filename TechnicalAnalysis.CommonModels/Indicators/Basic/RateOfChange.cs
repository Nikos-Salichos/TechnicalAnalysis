using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class RateOfChange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; }
        public double? Value { get; init; }
    }
}
