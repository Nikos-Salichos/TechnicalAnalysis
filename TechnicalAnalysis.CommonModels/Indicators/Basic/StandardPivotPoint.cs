using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class StandardPivotPoint : BaseIndicator
    {
        public Timeframe Timeframe { get; init; }
        public decimal? PivotPoint { get; init; }
        public decimal? Support1 { get; init; }
        public decimal? Support2 { get; init; }
        public decimal? Support3 { get; init; }
        public decimal? Resistance1 { get; init; }
        public decimal? Resistance2 { get; init; }
        public decimal? Resistance3 { get; init; }

        public StandardPivotPoint(long candlestickId)
           : base(candlestickId)
        {

        }
    }
}
