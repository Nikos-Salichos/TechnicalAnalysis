using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IPostgreSqlRepository
    {
        Task<IResult<IEnumerable<ProviderSynchronization>, string>> GetProvidersAsync();
        Task<IResult<IEnumerable<Candlestick>, string>> GetCandlesticksAsync();
        Task<IResult<IEnumerable<Pair>, string>> GetPairsAsync();
        Task<IResult<IEnumerable<Asset>, string>> GetAssetsAsync();
        Task<IResult<IEnumerable<Pool>, string>> GetPoolsAsync();
        Task<IResult<IEnumerable<DexCandlestick>, string>> GetDexCandlestickssAsync();
        Task InsertPairsAsync(IEnumerable<Pair> pairs);
        Task InsertCandlesticksAsync(IEnumerable<Candlestick> candlesticks);
        Task InsertAssetsAsync(IEnumerable<Asset> assets);
        Task UpdateProviderPairAssetSyncInfoAsync(ProviderPairAssetSyncInfo providerPairAssetSyncInfos);
        Task UpdateProviderCandlestickSyncInfoAsync(ProviderCandlestickSyncInfo providerCandlestickSyncInfos);
        Task InsertPoolsAsync(IEnumerable<Pool> pools);
        Task InsertDexCandlesticksAsync(IEnumerable<DexCandlestick> candlesticks);
        Task DeletePoolsByIdsAsync(IEnumerable<long> ids);
        Task DeleteDexCandlesticksByIdsAsync(IEnumerable<long> ids);
        Task DeleteTokensByIdsAsync(IEnumerable<long> ids);
        Task DeleteAssetsAsync();
    }
}
