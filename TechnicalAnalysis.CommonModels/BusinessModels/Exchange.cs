using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class Exchange : BaseEntity
    {
        public string? Name { get; init; }
        public int Code { get; init; }
        public DateTime LastAssetSync { get; set; }
        public DateTime LastPairSync { get; set; }
        public DateTime LastCandlestickSync { get; set; }
    }
}
