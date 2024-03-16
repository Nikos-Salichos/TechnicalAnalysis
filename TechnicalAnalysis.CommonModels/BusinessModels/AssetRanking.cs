using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class AssetRanking
    {
        public string Symbol { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
        public AssetType AssetType { get; init; }
        public DataProvider DataProvider { get; init; }
    }
}
