namespace TechnicalAnalysis.CommonModels.BaseClasses
{
    public abstract class BaseIndicator(long candlestickId)
    {
        public long CandlestickId { get; init; } = candlestickId;
    }
}
