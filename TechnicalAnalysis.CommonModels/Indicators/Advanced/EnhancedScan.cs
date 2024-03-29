using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class EnhancedScan(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool EnhancedScanIsLong { get; set; }
        public bool EnhancedScanIsShort { get; set; }
        public int OrderOfLongSignal { get; init; }
        public int OrderOfShortSignal { get; init; }
    }
}
