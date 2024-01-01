using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class VerticalHorizontalFilter(long candlestickId, long period, double value) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; } = period;
        public double Value { get; init; } = value;
    }
}
