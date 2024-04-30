using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Stochastic(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required double? OscillatorK { get; init; }
        public required double? SignalD { get; init; }
        public required double? PercentJ { get; init; }
    }
}
