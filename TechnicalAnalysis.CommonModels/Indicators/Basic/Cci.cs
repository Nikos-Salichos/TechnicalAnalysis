using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Cci(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required double? Value { get; init; }
        public required double? Overbought { get; init; }
        public required double? Oversold { get; init; }
    }
}
