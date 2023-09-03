using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class RateOfChange : BaseIndicator
    {
        public long Period { get; init; }
        public double? Value { get; init; }
        public RateOfChange(long candlestickId) : base(candlestickId)
        {
        }
    }
}
