using TechnicalAnalysis.CommonModels.Indicators.Advanced;

namespace TechnicalAnalysis.Domain.Contracts.Output
{
    public class EnhancedScanGroup
    {
        public DateTime CandlestickCloseDate { get; set; }
        public List<EnhancedScan> EnhancedScans { get; set; } = [];
    }
}
