using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class FractalRange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required int NumberOfFractalsInNumber { get; init; }
        public required decimal? HighestPriceOfFractals { get; init; }
        public required decimal? LowestPriceOfFractals { get; init; }
    }
}