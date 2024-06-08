using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class PairExtended : BaseEntity, IEquatable<PairExtended>
    {
        public string? Symbol { get; set; }
        public string? ContractAddress { get; init; }
        public long BaseAssetId { get; set; }
        public long QuoteAssetId { get; set; }
        public string? BaseAssetContract { get; init; }
        public string? QuoteAssetContract { get; init; }
        public string? BaseAssetName { get; set; }
        public string? QuoteAssetName { get; set; }
        public DataProvider Provider { get; set; }
        public bool IsActive { get; set; } = true;
        public bool AllCandles { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string? FeeTier { get; init; }
        public decimal? Fees { get; init; }
        public long? Liquidity { get; init; }
        public decimal? TotalValueLocked { get; init; }
        public decimal? Volume { get; init; }
        public long? NumberOfTrades { get; init; }
        public decimal? AllTimeHighPrice { get; set; }
        public decimal? AllTimeLowPrice { get; set; }
        public ProductType ProductType { get; init; }
        public List<CandlestickExtended> Candlesticks { get; set; } = [];

        public bool Equals(PairExtended? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other != null
                && BaseAssetId == other.BaseAssetId
                && QuoteAssetId == other.QuoteAssetId
                && Provider == other.Provider;
        }

        public override bool Equals(object? obj) => obj is not null && Equals(obj as PairExtended);

        public override int GetHashCode() => HashCode.Combine(BaseAssetId, QuoteAssetId, Provider);
    }
}
