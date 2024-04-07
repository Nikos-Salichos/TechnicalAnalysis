using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Aroon(long candlestickId, long period, double? aroonUp, double? aroonDown, double? oscillator) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; } = period;
        public double? AroonUp { get; init; } = aroonUp;
        public double? AroonDown { get; init; } = aroonDown;
        public double? Oscillator { get; init; } = oscillator;
    }
}
