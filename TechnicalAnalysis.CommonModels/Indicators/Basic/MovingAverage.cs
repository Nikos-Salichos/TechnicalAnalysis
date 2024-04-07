using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class MovingAverage(long candlestickId, double period, double? value, MovingAverageType movingAverageType) : BaseIndicator(candlestickId)
    {
        public double Period { get; init; } = period;
        public double? Value { get; init; } = value;
        public MovingAverageType MovingAverageType { get; init; } = movingAverageType;

        public static MovingAverage Create(long candlestickId, double period, double? value, MovingAverageType movingAverageType)
        {
            return new MovingAverage(candlestickId, period, value, movingAverageType);
        }
    }
}
