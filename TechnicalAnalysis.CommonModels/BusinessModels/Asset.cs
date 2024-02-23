using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class Asset : BaseEntity
    {
        public string Symbol { get; init; } = string.Empty;
        public DateTime CreatedDate { get; } = DateTime.UtcNow;
        public AssetType AssetType { get; init; }

        public bool Equals(Asset? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other != null && Symbol == other.Symbol;
        }

        public override bool Equals(object? obj) => obj is not null && Equals(obj as Asset);

        public override int GetHashCode() => HashCode.Combine(Symbol);
    }
}
