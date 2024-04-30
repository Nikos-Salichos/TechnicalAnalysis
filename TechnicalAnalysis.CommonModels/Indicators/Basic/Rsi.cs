using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Rsi(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required double Overbought { get; init; } = Constants.RsiOverbought;
        public required double Oversold { get; init; } = Constants.RsiOversold;
        public required double? Value { get; init; }
        public required long NumberOfRsiLowerThanPreviousRsis { get; init; }
        public bool RsiChangedDirectionFromPreviousCandlestick { get; set; }
        public long NumberOfConsecutiveLowerRsi { get; set; }
        public long NumberOfConsecutiveHigherRsi { get; set; }
    }
}
