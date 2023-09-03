using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Macd : BaseIndicator
    {
        public double? MacdValue { get; init; }
        public double? Signal { get; init; }
        public double? Histogram { get; init; }
        public double? FastEma { get; init; }
        public double? SlowEma { get; init; }

        public Macd(long candlestickId, double? macdValue, double? signal, double? histogram, double? fastEma, double? slowEma)
            : base(candlestickId)
        {
            MacdValue = macdValue;
            Signal = signal;
            Histogram = histogram;
            FastEma = fastEma;
            SlowEma = slowEma;
        }

        public static Macd Create(long candlestickId, double? macdValue, double? signal, double? histogram, double? fastEma, double? slowEma)
        {
            return new Macd(candlestickId, macdValue, signal, histogram, fastEma, slowEma);
        }
    }
}
