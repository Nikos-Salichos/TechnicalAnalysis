using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class VixFix(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required decimal? Value { get; init; }
    }
}
