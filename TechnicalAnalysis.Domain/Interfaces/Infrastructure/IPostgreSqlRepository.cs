using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IPostgreSqlRepository
    {
        Task<Result<List<FearAndGreedModel>, string>> GetCryptoFearAndGreedIndexAsync();
        Task<Result<List<FearAndGreedModel>, string>> GetStockFearAndGreedIndexAsync();
        Task<Result<List<ProviderSynchronization>, string>> GetProvidersAsync();
        Task<Result<List<Candlestick>, string>> GetCandlesticksAsync();
        Task<Result<List<Pair>, string>> GetPairsAsync();
        Task<Result<List<Asset>, string>> GetAssetsAsync();
        Task<Result<List<AssetRanking>, string>> GetCoinPaprikaAssetsAsync();
        Task<Result<List<PoolEntity>, string>> GetPoolsAsync();
        Task<Result<List<DexCandlestick>, string>> GetDexCandlesticksAsync();
        Task InsertCryptoFearAndGreedIndex(List<FearAndGreedModel> fearAndGreedModels);
        Task InsertStockFearAndGreedIndex(List<FearAndGreedModel> fearAndGreedModels);
        Task InsertPairsAsync(List<Pair> pairs);
        Task InsertCandlesticksAsync(List<Candlestick> candlesticks);
        Task InsertAssetsAsync(List<Asset> assets);
        Task InsertCoinPaprikaAssetsAsync(List<AssetRanking> assets);
        Task UpdateProviderPairAssetSyncInfoAsync(ProviderPairAssetSyncInfo providerPairAssetSyncInfo);
        Task UpdateProviderCandlestickSyncInfoAsync(ProviderCandlestickSyncInfo providerCandlestickSyncInfo);
        Task InsertPoolsAsync(List<PoolEntity> pools);
        Task InsertDexCandlesticksAsync(List<DexCandlestick> candlesticks);
        Task<Result<string, string>> DeleteEntitiesByIdsAsync<T>(List<long> ids, string tableName);
    }
}
