using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class FractalLowest(long candlestickId, long consecutive, PriceType priceType, decimal? price) : BaseIndicator(candlestickId)
    {
        public long Consecutive { get; init; } = consecutive;
        public decimal? Price { get; init; } = price;
        public PriceType PriceType { get; init; } = priceType;
    }
}
