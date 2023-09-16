using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IPostgreSqlRepository
    {
        Task<IResult<IEnumerable<ProviderSynchronization>, string>> GetProviders();
        Task<IResult<IEnumerable<Candlestick>, string>> GetCandlesticks();
        Task<IResult<IEnumerable<Pair>, string>> GetPairs();
        Task<IResult<IEnumerable<Asset>, string>> GetAssets();
        Task<IResult<IEnumerable<Pool>, string>> GetPools();
        Task<IResult<IEnumerable<DexCandlestick>, string>> GetDexCandlesticks();
        Task InsertPairs(IEnumerable<Pair> pairs);
        Task InsertCandlesticks(IEnumerable<Candlestick> candlesticks);
        Task InsertAssets(IEnumerable<Asset> assets);
        Task UpdateProviderPairAssetSyncInfo(ProviderPairAssetSyncInfo providerPairAssetSyncInfos);
        Task UpdateProviderCandlestickSyncInfo(ProviderCandlestickSyncInfo providerCandlestickSyncInfos);
        Task InsertPools(IEnumerable<Pool> pools);
        Task InsertDexCandlesticks(IEnumerable<DexCandlestick> candlesticks);
        Task DeletePoolsByIds(IEnumerable<long> ids);
        Task DeleteDexCandlesticksByIds(IEnumerable<long> ids);
        Task DeleteTokensByIds(IEnumerable<long> ids);
    }
}
