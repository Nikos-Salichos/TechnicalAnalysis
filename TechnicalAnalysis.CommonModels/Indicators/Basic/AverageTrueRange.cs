using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class AverageTrueRange(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required decimal? TrueRange { get; init; }
        public required decimal? AverageTrueRangeValue { get; init; }
        public required decimal? AverageTrueRangePercent { get; init; }
    }
}
