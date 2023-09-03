using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class DonchianChannel : BaseIndicator
    {
        public long Period { get; init; }
        public decimal? UpperBand { get; init; }
        public decimal? Centerline { get; init; }
        public decimal? LowerBand { get; init; }
        public decimal? Width { get; init; }

        public DonchianChannel(long candlestickId, long period, decimal? upperBand, decimal? centerline, decimal? lowerBand, decimal? width)
            : base(candlestickId)
        {
            Period = period;
            UpperBand = upperBand;
            Centerline = centerline;
            LowerBand = lowerBand;
            Width = width;
        }

        public static DonchianChannel Create(long candlestickId, long period, decimal? upperBand, decimal? centerline, decimal? lowerBand, decimal? width)
        {
            return new DonchianChannel(candlestickId, period, upperBand, centerline, lowerBand, width);
        }
    }
}
