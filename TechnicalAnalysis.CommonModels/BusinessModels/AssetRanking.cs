using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class AssetRanking
    {
        public string? Symbol { get; init; }
        public string? Name { get; init; }
        public DateTime CreatedDate { get; init; }
        public ProductType AssetType { get; init; }
        public DataProvider DataProvider { get; init; }
    }
}
