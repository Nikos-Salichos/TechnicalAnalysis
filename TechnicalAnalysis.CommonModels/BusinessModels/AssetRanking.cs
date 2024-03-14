using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class AssetRanking : IEquatable<AssetRanking>
    {
        public string Symbol { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
        public AssetType AssetType { get; init; }
        public DataProvider DataProvider { get; init; }

        public bool Equals(AssetRanking? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other != null && Name == other.Name;
        }

        public override bool Equals(object? obj) => obj is not null && Equals(obj as AssetRanking);

        public override int GetHashCode() => HashCode.Combine(Name);
    }
}
