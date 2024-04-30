using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class StandardPivotPoint(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required Timeframe Timeframe { get; init; }
        public required decimal? PivotPoint { get; init; }
        public required decimal? Support1 { get; init; }
        public required decimal? Support2 { get; init; }
        public required decimal? Support3 { get; init; }
        public required decimal? Resistance1 { get; init; }
        public required decimal? Resistance2 { get; init; }
        public required decimal? Resistance3 { get; init; }
    }
}
