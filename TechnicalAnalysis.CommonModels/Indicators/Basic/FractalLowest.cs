using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class FractalLowest(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Consecutive { get; init; }
        public required decimal? Price { get; init; }
        public required PriceType PriceType { get; init; }
    }
}
