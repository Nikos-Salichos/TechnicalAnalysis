using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class StandardDeviation(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required double? StadardDeviationValue { get; init; }
        public required double? Mean { get; init; }
        public required double? ZScore { get; init; }
        public required double? StdDevSma { get; init; }
        public required long Period { get; init; }
    }
}
