using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class PairExtended : BaseEntity, IEquatable<PairExtended>
    {
        public string Symbol { get; set; } = string.Empty;
        public string ContractAddress { get; init; } = string.Empty;
        public long BaseAssetId { get; set; }
        public long QuoteAssetId { get; set; }
        public string BaseAssetContract { get; init; } = string.Empty;
        public string QuoteAssetContract { get; init; } = string.Empty;
        public string BaseAssetName { get; set; } = string.Empty;
        public string QuoteAssetName { get; set; } = string.Empty;
        public DataProvider Provider { get; set; }
        public bool IsActive { get; set; } = true;
        public bool AllCandles { get; init; }
        public DateTime CreatedAt { get; init; }
        public string FeeTier { get; init; } = string.Empty;
        public decimal? Fees { get; init; }
        public long? Liquidity { get; init; }
        public decimal? TotalValueLocked { get; init; }
        public decimal? Volume { get; init; }
        public long? NumberOfTrades { get; init; }
        public List<CandlestickExtended> Candlesticks { get; set; } = [];
        public bool HasCalculateDailyTechnicalAnalysis { get; set; }

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
