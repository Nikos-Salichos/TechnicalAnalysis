using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Rsi : BaseIndicator
    {
        public long Period { get; init; }
        public double Overbought { get; init; } = 80;
        public double Oversold { get; init; } = 20;
        public double? Value { get; init; }
        public long NumberOfRsiLowerThanPreviousRsis { get; init; }
        public bool RsiChangedDirectionFromPreviousCandlestick { get; set; }

        public Rsi(long candlestickId) : base(candlestickId)
        {
        }
    }
}
