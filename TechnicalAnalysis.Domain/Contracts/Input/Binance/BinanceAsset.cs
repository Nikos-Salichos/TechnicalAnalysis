namespace TechnicalAnalysis.Domain.Contracts.Input.Binance
{
    public class BinanceAsset
    {
        public long Id { get; init; }
        public string Asset { get; init; }
    }

    public class BinanceAssetComparer : IEqualityComparer<BinanceAsset>
    {
        public bool Equals(BinanceAsset? x, BinanceAsset? y)
        {
            return x?.Asset == y?.Asset;
        }

        public int GetHashCode(BinanceAsset obj)
        {
            return obj.Asset.GetHashCode();
        }
    }
}
