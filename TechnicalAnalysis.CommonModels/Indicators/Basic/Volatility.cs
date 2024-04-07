using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Volatility : BaseIndicator
    {
        public double VolatilityValue { get; init; }
        public long Period { get; init; }

        public Volatility(long candlestickId, double volatilityValue, long period)
           : base(candlestickId)
        {
            Period = period;
            VolatilityValue = volatilityValue;
            Period = period;
        }
    }
}
