using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Volatility(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required double VolatilityValue { get; init; }
        public required long Period { get; init; }
    }
}
