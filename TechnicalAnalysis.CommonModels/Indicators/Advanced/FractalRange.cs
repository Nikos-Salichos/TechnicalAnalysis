using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class FractalRange : BaseIndicator
    {
        public int NumberOfFractalsInNumber { get; init; }
        public decimal? HighestPriceOfFractals { get; init; }
        public decimal? LowestPriceOfFractals { get; init; }

        public FractalRange(long candlestickId) : base(candlestickId)
        {
        }
    }
}