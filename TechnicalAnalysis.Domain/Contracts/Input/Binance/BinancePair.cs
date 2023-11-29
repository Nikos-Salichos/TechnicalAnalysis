using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Contracts.Input.Binance
{
    public class BinancePair
    {
        public long Id { get; init; }
        public string Pair { get; init; } = string.Empty;
        public long BaseAssetId { get; init; }
        public long QuoteAssetId { get; init; }
        public DataProvider Provider { get; init; }
        public bool IsActive { get; init; }
        public bool AllCandles { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.Now;
        public List<BinanceCandlestick> BinanceCandlesticks { get; set; } = new List<BinanceCandlestick>();
    }
}
