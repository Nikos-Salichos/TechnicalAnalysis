namespace TechnicalAnalysis.Domain.Contracts.Input.Binance
{
    public sealed class BinanceAsset : IEquatable<BinanceAsset>
    {
        public long Id { get; init; }
        public string Asset { get; init; } = string.Empty;

        public bool Equals(BinanceAsset? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other != null && Asset == other.Asset;
        }

        public override bool Equals(object? obj) => obj is not null && Equals(obj as BinanceAsset);

        public override int GetHashCode() => HashCode.Combine(Asset);
    }
}
