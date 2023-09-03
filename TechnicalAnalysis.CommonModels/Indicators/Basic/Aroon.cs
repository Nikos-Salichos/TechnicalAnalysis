using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Aroon : BaseIndicator
    {
        public long Period { get; init; }
        public double? AroonUp { get; init; }
        public double? AroonDown { get; init; }
        public double? Oscillator { get; init; }

        public Aroon(long candlestickId, long period, double? aroonUp, double? aroonDown, double? oscillator)
            : base(candlestickId)
        {
            Period = period;
            AroonUp = aroonUp;
            AroonDown = aroonDown;
            Oscillator = oscillator;
        }

        public static Aroon Create(long candlestickId, long period, double? aroonUp, double? aroonDown, double? oscillator)
            => new Aroon(candlestickId, period, aroonUp, aroonDown, oscillator);
    }

}
