using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class AverageRange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public decimal? Value { get; init; }
        public long Period { get; init; }
    }
}
