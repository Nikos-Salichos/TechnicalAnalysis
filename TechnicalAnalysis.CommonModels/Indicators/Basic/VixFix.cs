using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class VixFix(long candlestickId) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; }
        public decimal? Value { get; init; }
    }
}
