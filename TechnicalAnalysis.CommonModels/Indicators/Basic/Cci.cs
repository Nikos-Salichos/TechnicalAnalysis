using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Cci(long candlestickId, long period, double? value, double? overbought, double? oversold) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; } = period;
        public double? Value { get; init; } = value;
        public double? Overbought { get; init; } = overbought;
        public double? Oversold { get; init; } = oversold;

        public static Cci Create(long candlestickId, long period, double? value, double? overbought, double? oversold)
        {
            return new Cci(candlestickId, period, value, overbought, oversold);
        }
    }
}
