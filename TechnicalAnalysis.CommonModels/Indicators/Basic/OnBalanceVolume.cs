using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class OnBalanceVolume(long candlestickId, double value) : BaseIndicator(candlestickId)
    {
        public double Value { get; init; } = value;
    }
}
