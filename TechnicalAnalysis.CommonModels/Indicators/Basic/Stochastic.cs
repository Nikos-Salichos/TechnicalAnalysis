using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Stochastic : BaseIndicator
    {
        public double? OscillatorK { get; init; }
        public double? SignalD { get; init; }
        public double? PercentJ { get; init; }

        public Stochastic(long candlestickId, double? oscillatorK, double? signalD, double? percentJ)
            : base(candlestickId)
        {
            OscillatorK = oscillatorK;
            SignalD = signalD;
            PercentJ = percentJ;
        }

        public static Stochastic Create(long candlestickId, double? oscillatorK, double? signalD, double? percentJ)
        {
            return new Stochastic(candlestickId, oscillatorK, signalD, percentJ);
        }
    }
}
