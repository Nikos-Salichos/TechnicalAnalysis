using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Rsi : BaseIndicator
    {
        public long Period { get; init; }
        public double Overbought { get; init; } = Constants.RsiOverbought;
        public double Oversold { get; init; } = Constants.RsiOversold;
        public double? Value { get; init; }
        public long NumberOfRsiLowerThanPreviousRsis { get; init; }
        public bool RsiChangedDirectionFromPreviousCandlestick { get; set; }
        public long NumberOfConsecutiveLowerRsi { get; set; }
        public long NumberOfConsecutiveHigherRsi { get; set; }

        public Rsi(long candlestickId) : base(candlestickId)
        {
        }
    }
}
