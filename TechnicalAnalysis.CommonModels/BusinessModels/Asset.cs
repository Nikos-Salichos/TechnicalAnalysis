using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class Asset : BaseEntity, IEquatable<Asset>
    {
        public required string? Symbol { get; init; }
        public DateTime CreatedDate { get; } = DateTime.UtcNow;
        public required ProductType ProductType { get; init; }

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
