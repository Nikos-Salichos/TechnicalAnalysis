using System.Text.Json.Serialization;
using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class PairExtended : BaseEntity
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
        public bool IsActive { get; set; }
        public bool AllCandles { get; init; }
        public DateTime CreatedAt { get; init; }
        public string FeeTier { get; init; } = string.Empty;
        public decimal? Fees { get; init; }
        public long? Liquidity { get; init; }
        public decimal? TotalValueLocked { get; init; }
        public decimal? Volume { get; init; }
        public long? NumberOfTrades { get; init; }
        public List<CandlestickExtended> Candlesticks { get; set; } = new List<CandlestickExtended>();

        [JsonConstructor]
        public PairExtended()
        {

        }
    }


    public class PairExtendedEqualityComparer : IEqualityComparer<PairExtended>
    {
        public bool Equals(PairExtended? x, PairExtended? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.BaseAssetId == y.BaseAssetId &&
                   x.QuoteAssetId == y.QuoteAssetId &&
                   x.Provider == y.Provider;
        }

        public int GetHashCode(PairExtended obj)
        {
            return obj.BaseAssetId.GetHashCode() ^
                   obj.QuoteAssetId.GetHashCode() ^
                   obj.Provider.GetHashCode();
        }
    }

}
