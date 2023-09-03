using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Highest : BaseIndicator
    {
        public long Period { get; init; }
        public decimal? Value { get; init; }
        public PriceType PriceType { get; init; }

        public Highest(long candlestickId) : base(candlestickId)
        {
        }
    }
}
