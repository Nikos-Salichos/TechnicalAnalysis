using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class MovingAverage : BaseIndicator
    {
        public double Period { get; init; }
        public double? Value { get; init; }
        public MovingAverageType MovingAverageType { get; init; }

        public MovingAverage(long candlestickId, double period, double? value, MovingAverageType movingAverageType)
            : base(candlestickId)
        {
            Period = period;
            Value = value;
            MovingAverageType = movingAverageType;
        }

        public static MovingAverage Create(long candlestickId, double period, double? value, MovingAverageType movingAverageType)
        {
            return new MovingAverage(candlestickId, period, value, movingAverageType);
        }
    }

}
