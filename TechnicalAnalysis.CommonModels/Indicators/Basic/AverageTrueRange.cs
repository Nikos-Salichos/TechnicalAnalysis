using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class AverageTrueRange(long candlestickId,
        long period,
        decimal? trueRange,
        decimal? averageTrueRangeValue,
        decimal? averageTrueRangePercent) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; } = period;
        public decimal? TrueRange { get; init; } = trueRange;
        public decimal? AverageTrueRangeValue { get; init; } = averageTrueRangeValue;
        public decimal? AverageTrueRangePercent { get; init; } = averageTrueRangePercent;
    }
}
