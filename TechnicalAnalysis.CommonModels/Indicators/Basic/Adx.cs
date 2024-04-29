using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Adx(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required double? PlusDi { get; init; }
        public required double? MinusDi { get; init; }
        public required double? Value { get; init; }
        public required double? Adxr { get; init; }
    }
}
