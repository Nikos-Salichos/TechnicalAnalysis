namespace TechnicalAnalysis.CommonModels.BaseClasses
{
    public abstract class BaseIndicator
    {
        public long CandlestickId { get; init; }

        protected BaseIndicator(long candlestickId)
        {
            CandlestickId = candlestickId;
        }
    }
}
