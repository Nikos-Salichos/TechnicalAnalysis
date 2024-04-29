using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class AverageRange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required decimal? Value { get; init; }
        public required long Period { get; init; }
    }
}
