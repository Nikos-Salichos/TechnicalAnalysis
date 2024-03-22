using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IPostgreSqlRepository
    {
        Task<IResult<IEnumerable<CryptoFearAndGreedData>, string>> GetCryptoFearAndGreedIndexAsync();
        Task<IResult<IEnumerable<ProviderSynchronization>, string>> GetProvidersAsync();
        Task<IResult<IEnumerable<Candlestick>, string>> GetCandlesticksAsync();
        Task<IResult<IEnumerable<Pair>, string>> GetPairsAsync();
        Task<IResult<IEnumerable<Asset>, string>> GetAssetsAsync();
        Task<IResult<IEnumerable<AssetRanking>, string>> GetCoinPaprikaAssetsAsync();
        Task<IResult<IEnumerable<Pool>, string>> GetPoolsAsync();
        Task<IResult<IEnumerable<DexCandlestick>, string>> GetDexCandlesticksAsync();
        Task InsertCryptoFearAndGreedIndex(IEnumerable<CryptoFearAndGreedData> indexes);
        Task InsertPairsAsync(IEnumerable<Pair> pairs);
        Task InsertCandlesticksAsync(IEnumerable<Candlestick> candlesticks);
        Task<IResult<string, string>> InsertAssetsAsync(IEnumerable<Asset> assets);
        Task<IResult<string, string>> InsertCoinPaprikaAssetsAsync(IEnumerable<AssetRanking> assets);
        Task UpdateProviderPairAssetSyncInfoAsync(ProviderPairAssetSyncInfo providerPairAssetSyncInfo);
        Task UpdateProviderCandlestickSyncInfoAsync(ProviderCandlestickSyncInfo providerCandlestickSyncInfo);
        Task InsertPoolsAsync(IEnumerable<Pool> pools);
        Task InsertDexCandlesticksAsync(IEnumerable<DexCandlestick> candlesticks);
        Task DeleteEntitiesByIdsAsync<T>(IEnumerable<long> ids, string tableName);
    }
}
