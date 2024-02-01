using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class FractalRange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public int NumberOfFractalsInNumber { get; init; }
        public decimal? HighestPriceOfFractals { get; init; }
        public decimal? LowestPriceOfFractals { get; init; }
    }
}