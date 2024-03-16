using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public sealed class VerticalHorizontalFilterRange : BaseIndicator
    {
        public double RangeLimit { get; init; }
        public int NumberOfCandlesticksInRange { get; init; }

        public VerticalHorizontalFilterRange(long candlestickId, double rangeLimit, int numberOfCandlesticksInRange)
            : base(candlestickId)
        {
            RangeLimit = rangeLimit;
            NumberOfCandlesticksInRange = numberOfCandlesticksInRange;
        }
    }
}