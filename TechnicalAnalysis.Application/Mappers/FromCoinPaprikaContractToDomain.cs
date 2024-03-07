using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.CoinPaprika;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromCoinPaprikaContractToDomain
    {
        private static AssetRanking ToDomain(this CoinPaprikaAssetContract asset)
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
                Name = asset.Name,
                AssetType = assetType,
                DataProvider = DataProvider.CoinPaprika,
                CreatedDate = (asset.CreatedAt == DateTime.MinValue) ? DateTime.UtcNow : asset.CreatedAt,
            };
        }

        public static IEnumerable<AssetRanking> ToDomain(this IEnumerable<CoinPaprikaAssetContract> assets)
            => assets.Select(p => p.ToDomain()).ToList();
    }
}
