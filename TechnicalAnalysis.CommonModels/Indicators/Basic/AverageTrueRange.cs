using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class AverageTrueRange : BaseIndicator
    {
        public long Period { get; init; }
        public decimal? TrueRange { get; init; }
        public decimal? AverageTrueRangeValue { get; init; }
        public decimal? AverageTrueRangePercent { get; init; } // (ATR/Price)*100

        public AverageTrueRange(long candlestickId,
            long period,
            decimal? trueRange,
            decimal? averageTrueRangeValue,
            decimal? averageTrueRangePercent)
            : base(candlestickId)
        {
            Period = period;
            TrueRange = trueRange;
            AverageTrueRangeValue = averageTrueRangeValue;
            AverageTrueRangePercent = averageTrueRangePercent;
        }
    }
}
