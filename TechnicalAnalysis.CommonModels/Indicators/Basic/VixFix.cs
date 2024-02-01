using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class VixFix : BaseIndicator
    {
        public long Period { get; init; }
        public decimal? Value { get; init; }
        public VixFix(long candlestickId) : base(candlestickId)
        {
        }
    }
}
