using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class KeltnerChannel : BaseIndicator
    {
        public long Period { get; init; }
        public double? UpperBand { get; init; }
        public double? Centerline { get; init; }
        public double? LowerBand { get; init; }
        public double? Width { get; init; }

        public KeltnerChannel(long candlestickId, long period, double? upperBand, double? centerline, double? lowerBand, double? width)
            : base(candlestickId)
        {
            Period = period;
            UpperBand = upperBand;
            Centerline = centerline;
            LowerBand = lowerBand;
            Width = width;
        }

        public static KeltnerChannel Create(long candlestickId, long period, double? upperBand, double? centerline, double? lowerBand, double? width)
        {
            return new KeltnerChannel(candlestickId, period, upperBand, centerline, lowerBand, width);
        }

    }
}
