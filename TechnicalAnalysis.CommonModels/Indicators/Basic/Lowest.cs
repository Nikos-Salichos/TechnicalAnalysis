using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Lowest : BaseIndicator
    {
        public long Period { get; init; }
        public decimal? Value { get; init; }
        public PriceType PriceType { get; init; }

        public Lowest(long candlestickId) : base(candlestickId)
        { }

    }
}
