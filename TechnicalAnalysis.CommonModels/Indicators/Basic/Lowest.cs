using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Lowest(long candlestickId) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; }
        public decimal? Value { get; init; }
        public PriceType PriceType { get; init; }
    }
}
