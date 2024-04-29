using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class DonchianChannel(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required decimal? UpperBand { get; init; }
        public required decimal? Centerline { get; init; }
        public required decimal? LowerBand { get; init; }
        public required decimal? Width { get; init; }
    }
}
