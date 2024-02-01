using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Cci : BaseIndicator
    {
        public long Period { get; init; }
        public double? Value { get; init; }
        public double? Overbought { get; init; } = 100;
        public double? Oversold { get; init; } = -100;

        public Cci(long candlestickId, long period, double? value, double? overbought, double? oversold)
           : base(candlestickId)
        {
            Period = period;
            Value = value;
            Overbought = overbought;
            Oversold = oversold;
        }

        public static Cci Create(long candlestickId, long period, double? value, double? overbought, double? oversold)
        {
            return new Cci(candlestickId, period, value, overbought, oversold);
        }
    }

}
