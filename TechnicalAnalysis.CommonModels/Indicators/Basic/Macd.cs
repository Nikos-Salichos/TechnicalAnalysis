using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Macd(long candlestickId, double? macdValue, double? signal, double? histogram, double? fastEma, double? slowEma) : BaseIndicator(candlestickId)
    {
        public double? MacdValue { get; init; } = macdValue;
        public double? Signal { get; init; } = signal;
        public double? Histogram { get; init; } = histogram;
        public double? FastEma { get; init; } = fastEma;
        public double? SlowEma { get; init; } = slowEma;

        public static Macd Create(long candlestickId, double? macdValue, double? signal, double? histogram, double? fastEma, double? slowEma)
        {
            return new Macd(candlestickId, macdValue, signal, histogram, fastEma, slowEma);
        }
    }
}
