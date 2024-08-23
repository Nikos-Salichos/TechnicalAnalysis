using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class AssetRanking
    {
        public required string? Symbol { get; init; }
        public required string? Name { get; init; }
        public required DateTime CreatedDate { get; init; } = DateTime.UtcNow;
        public required ProductType ProductType { get; init; }
        public required DataProvider DataProvider { get; init; }
    }
}
