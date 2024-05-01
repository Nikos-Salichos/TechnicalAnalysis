using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Macd(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required double? MacdValue { get; init; }
        public required double? Signal { get; init; }
        public required double? Histogram { get; init; }
        public required double? FastEma { get; init; }
        public required double? SlowEma { get; init; }
    }
}
