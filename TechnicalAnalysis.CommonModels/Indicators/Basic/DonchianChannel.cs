using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class DonchianChannel(long candlestickId, long period, decimal? upperBand, decimal? centerline, decimal? lowerBand, decimal? width) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; } = period;
        public decimal? UpperBand { get; init; } = upperBand;
        public decimal? Centerline { get; init; } = centerline;
        public decimal? LowerBand { get; init; } = lowerBand;
        public decimal? Width { get; init; } = width;

        public static DonchianChannel Create(long candlestickId, long period, decimal? upperBand, decimal? centerline, decimal? lowerBand, decimal? width)
        {
            return new DonchianChannel(candlestickId, period, upperBand, centerline, lowerBand, width);
        }
    }
}
