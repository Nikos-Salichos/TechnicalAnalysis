using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public sealed class VerticalHorizontalFilterRange(long candlestickId)
        : BaseIndicator(candlestickId)
    {
        public required double RangeLimit { get; init; }
        public required int NumberOfCandlesticksInRange { get; init; }
    }
}