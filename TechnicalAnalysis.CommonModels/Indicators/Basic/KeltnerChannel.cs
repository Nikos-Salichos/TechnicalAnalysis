using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class KeltnerChannel : BaseIndicator
    {
        public long Period { get; init; }
        public double? UpperBand { get; init; }
        public double? Centerline { get; init; }
        public double? LowerBand { get; init; }
        public double? Width
            => UpperBand.HasValue && LowerBand.HasValue
            ? Math.Abs(UpperBand.Value - LowerBand.Value)
            : null;

        public KeltnerChannel(long candlestickId, long period, double? upperBand, double? centerline, double? lowerBand)
            : base(candlestickId)
        {
            Period = period;
            UpperBand = upperBand;
            Centerline = centerline;
            LowerBand = lowerBand;
        }

        public static KeltnerChannel Create(long candlestickId, long period, double? upperBand, double? centerline, double? lowerBand)
        {
            return new KeltnerChannel(candlestickId, period, upperBand, centerline, lowerBand);
        }

    }
}
