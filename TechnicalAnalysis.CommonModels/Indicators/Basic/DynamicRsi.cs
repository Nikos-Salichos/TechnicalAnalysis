using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class DynamicRsi(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required double Overbought { get; init; }
        public required double Oversold { get; init; }
        public required double? Value { get; init; }
        public required long NumberOfRsiLowerThanPreviousRsis { get; init; }
        public bool RsiChangedDirectionFromPreviousCandlestick { get; set; }
        public long NumberOfConsecutiveLowerRsi { get; set; }
        public long NumberOfConsecutiveHigherRsi { get; set; }
    }
}
