using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Stochastic(long candlestickId, double? oscillatorK, double? signalD, double? percentJ) : BaseIndicator(candlestickId)
    {
        public double? OscillatorK { get; init; } = oscillatorK;
        public double? SignalD { get; init; } = signalD;
        public double? PercentJ { get; init; } = percentJ;

        public static Stochastic Create(long candlestickId, double? oscillatorK, double? signalD, double? percentJ)
        {
            return new Stochastic(candlestickId, oscillatorK, signalD, percentJ);
        }
    }
}
