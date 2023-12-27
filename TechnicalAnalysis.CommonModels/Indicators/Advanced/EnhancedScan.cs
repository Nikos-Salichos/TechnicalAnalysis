using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class EnhancedScan(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool EnhancedScanIsBuy { get; init; }

        public bool EnhancedScanIsSell { get; init; }

        public int OrderOfSignal { get; init; }
    }
}
