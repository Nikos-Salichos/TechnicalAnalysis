using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Aroon(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required double? AroonUp { get; init; }
        public required double? AroonDown { get; init; }
        public required double? Oscillator { get; init; }
    }
}
