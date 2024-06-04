using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Entities
{
    public class PoolEntity : BaseEntity
    {
        public DataProvider Provider { get; init; }
        public string? PoolContract { get; init; }
        public long Token0Id { get; init; }
        public string? Token0Contract { get; init; }
        public long Token1Id { get; init; }
        public string? Token1Contract { get; init; }
        public string? FeeTier { get; init; }
        public decimal? Fees { get; init; }
        public long? Liquidity { get; init; }
        public decimal? TotalValueLocked { get; init; }
        public decimal? Volume { get; init; }
        public long? NumberOfTrades { get; init; }
        public bool IsActive { get; init; }
    }
}
