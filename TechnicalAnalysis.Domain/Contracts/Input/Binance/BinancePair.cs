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

    public class BinancePairEqualityComparer : IEqualityComparer<BinancePair>
    {
        public bool Equals(BinancePair? x, BinancePair? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.BaseAssetId == y.BaseAssetId &&
                   x.QuoteAssetId == y.QuoteAssetId &&
                   x.Provider == y.Provider;
        }

        public int GetHashCode(BinancePair obj)
        {
            return obj.BaseAssetId.GetHashCode() ^
                   obj.QuoteAssetId.GetHashCode() ^
                   obj.Provider.GetHashCode();
        }
    }
}
