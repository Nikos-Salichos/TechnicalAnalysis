using TechnicalAnalysis.CommonModels.Indicators.Advanced;

namespace TechnicalAnalysis.Domain.Contracts.Output
{
    public class EnhancedScanGroup
    {
        public DateTime CandlestickCloseDate { get; init; }
        public List<EnhancedScan> EnhancedScans { get; init; } = [];
        public decimal? CandlestickClosePrice { get; init; }
        public decimal? PercentageFromAllTimeHigh { get; set; }
        public decimal? DaysFromAllTimeHigh { get; set; }
    }
}
