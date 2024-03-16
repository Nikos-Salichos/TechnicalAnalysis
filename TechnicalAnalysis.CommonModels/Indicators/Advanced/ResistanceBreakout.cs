using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class ResistanceBreakout(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool IsBuy { get; set; }
        public bool ClosePosition { get; set; }
        public bool IsPositionClosed { get; set; }
        public DateTime ExitPositionDate { get; set; }
        public int OrderOfSignal { get; set; }
        public long? FlagPoleCandlestickId { get; set; }
        public decimal? ProfitInPoints { get; set; }
        public decimal? LossInPoints { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public decimal? ValueAtExit { get; set; }
        public decimal? ProfitInMoney { get; set; }
        public decimal? LossInMoney { get; set; }
    }
}