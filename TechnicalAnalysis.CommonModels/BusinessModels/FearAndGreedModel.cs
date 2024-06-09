using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class FearAndGreedModel : IEquatable<FearAndGreedModel>
    {
        public string? Value { get; init; }

        public ValueClassificationType ValueClassificationType { get; init; }

        public DateTime DateTime { get; init; } = DateTime.UtcNow;

        public bool Equals(FearAndGreedModel? other)
            => other != null && DateTime.Date == other.DateTime.Date;

        public override bool Equals(object? obj)
            => obj is FearAndGreedModel other && Equals(other);

        public override int GetHashCode() => DateTime.Date.GetHashCode();
    }
}
