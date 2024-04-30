using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class OnBalanceVolume(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required double Value { get; init; }
    }
}
