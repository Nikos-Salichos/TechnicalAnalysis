using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class MovingAverage(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required double Period { get; init; }
        public required double? Value { get; init; }
        public required MovingAverageType MovingAverageType { get; init; }
    }
}
