using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class FearAndGreedModel : IEquatable<FearAndGreedModel>
    {
        public required string? Value { get; init; }

        public required ValueClassificationType ValueClassificationType { get; init; }

        public required DateTime DateTime { get; init; } = DateTime.UtcNow;

        public bool Equals(FearAndGreedModel? other)
            => other != null && DateTime.Date == other.DateTime.Date;

        public override bool Equals(object? obj)
            => obj is FearAndGreedModel other && Equals(other);

        public override int GetHashCode() => DateTime.Date.GetHashCode();
    }
}
