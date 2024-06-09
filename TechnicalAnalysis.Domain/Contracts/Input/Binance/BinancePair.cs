using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Contracts.Input.Binance
{
    public class BinancePair
    {
        public long Id { get; init; }
        public string? Pair { get; init; }
        public long BaseAssetId { get; init; }
        public long QuoteAssetId { get; init; }
        public DataProvider Provider { get; init; }
        public bool IsActive { get; init; }
        public bool AllCandles { get; init; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public List<BinanceCandlestick> BinanceCandlesticks { get; set; } = [];
    }
}
