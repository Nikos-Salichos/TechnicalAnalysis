using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class ProviderCandlestickSyncInfo
    {
        public long CandlestickSyncInfoId { get; set; }
        public long ProviderId { get; set; }
        public long TimeframeId { get; set; }
        public Timeframe Timeframe { get; set; }
        public DateTime LastCandlestickSync { get; set; }
    }
}
