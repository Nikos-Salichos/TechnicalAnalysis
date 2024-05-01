using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class KeltnerChannel(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required double? UpperBand { get; init; }
        public required double? Centerline { get; init; }
        public required double? LowerBand { get; init; }
        public double? Width
            => UpperBand.HasValue && LowerBand.HasValue
            ? Math.Abs(UpperBand.Value - LowerBand.Value)
            : null;
    }
}
