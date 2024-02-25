using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Output.CoinPaprika;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromCoinPaprikaContractToDomain
    {
        private static Asset ToDomain(this CoinPaprikaAsset asset)
        {
            AssetType assetType = AssetType.Unknown;

            if (asset.Type is "coin")
            {
                assetType = AssetType.Layer1;
            }

            if (asset.Type == "token")
            {
                assetType = AssetType.Layer2;
            }

            return new()
            {
                Symbol = asset.Symbol,
                AssetType = assetType
            };
        }

        public static IEnumerable<Asset> ToDomain(this IEnumerable<CoinPaprikaAsset> assets)
            => assets.Select(p => p.ToDomain()).ToList();
    }
}
