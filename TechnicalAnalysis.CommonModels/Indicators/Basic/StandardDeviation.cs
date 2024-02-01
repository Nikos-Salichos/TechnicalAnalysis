using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class StandardDeviation : BaseIndicator
    {
        public double? StadardDeviationValue { get; init; }
        public double? Mean { get; init; }
        public double? ZScore { get; init; }
        public double? StdDevSma { get; init; }
        public long Period { get; init; }

        public StandardDeviation(long candlestickId)
           : base(candlestickId)
        { }
    }
}
