using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class FractalLowest : BaseIndicator
    {
        public long Consecutive { get; init; }
        public decimal? Price { get; init; }
        public PriceType PriceType { get; init; }

        public FractalLowest(long candlestickId, long concecutive, PriceType priceType, decimal? price)
            : base(candlestickId)
        {
            Consecutive = concecutive;
            PriceType = priceType;
            Price = price;
        }
    }
}
