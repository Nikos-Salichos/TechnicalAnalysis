using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class KeltnerChannel(long candlestickId, long period, double? upperBand, double? centerline, double? lowerBand) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; } = period;
        public double? UpperBand { get; init; } = upperBand;
        public double? Centerline { get; init; } = centerline;
        public double? LowerBand { get; init; } = lowerBand;
        public double? Width
            => UpperBand.HasValue && LowerBand.HasValue
            ? Math.Abs(UpperBand.Value - LowerBand.Value)
            : null;

        public static KeltnerChannel Create(long candlestickId, long period, double? upperBand, double? centerline, double? lowerBand)
        {
            return new KeltnerChannel(candlestickId, period, upperBand, centerline, lowerBand);
        }
    }
}
