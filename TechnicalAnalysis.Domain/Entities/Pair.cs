using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Entities
{
    public class Pair : BaseEntity
    {
        public string Symbol { get; init; }
        public long BaseAssetId { get; init; }
        public long QuoteAssetId { get; init; }
        public DataProvider Provider { get; init; }
        public bool IsActive { get; init; }
        public bool AllCandles { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
