using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Highest(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required decimal? Value { get; set; }
        public required PriceType PriceType { get; init; }
    }
}
