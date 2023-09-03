using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class AverageRange : BaseIndicator
    {
        public decimal? Value { get; init; }
        public long Period { get; init; }

        public AverageRange(long candlestickId) : base(candlestickId)
        {
        }
    }
}
