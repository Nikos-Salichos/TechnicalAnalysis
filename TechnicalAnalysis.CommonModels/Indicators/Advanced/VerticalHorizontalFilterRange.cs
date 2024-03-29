using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public sealed class VerticalHorizontalFilterRange(long candlestickId, double rangeLimit, int numberOfCandlesticksInRange)
        : BaseIndicator(candlestickId)
    {
        public double RangeLimit { get; init; } = rangeLimit;
        public int NumberOfCandlesticksInRange { get; init; } = numberOfCandlesticksInRange;
    }
}