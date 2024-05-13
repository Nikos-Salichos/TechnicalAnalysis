using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IPostgreSqlRepository
    {
        Task<IResult<List<FearAndGreedModel>, string>> GetCryptoFearAndGreedIndexAsync();
        Task<IResult<List<FearAndGreedModel>, string>> GetStockFearAndGreedIndexAsync();
        Task<IResult<List<ProviderSynchronization>, string>> GetProvidersAsync();
        Task<IResult<List<Candlestick>, string>> GetCandlesticksAsync();
        Task<IResult<List<Pair>, string>> GetPairsAsync();
        Task<IResult<List<Asset>, string>> GetAssetsAsync();
        Task<IResult<List<AssetRanking>, string>> GetCoinPaprikaAssetsAsync();
        Task<IResult<List<PoolEntity>, string>> GetPoolsAsync();
        Task<IResult<List<DexCandlestick>, string>> GetDexCandlesticksAsync();
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
        Task<IResult<string, string>> DeleteEntitiesByIdsAsync<T>(List<long> ids, string tableName);
    }
}
