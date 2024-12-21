using Microsoft.AspNetCore.Http;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Output;

namespace TechnicalAnalysis.Domain.Interfaces.Application
{
    public interface IAnalysisService
    {
        Task<List<EnhancedPairResult>> GetEnhancedPairResultsAsync(DataProvider provider, HttpContext? httpContext = null);
        Task<List<PairExtended>> GetIndicatorsByPairNamesAsync(List<string> pairNames, Timeframe timeframe, HttpContext? httpContext = null);
        Task<List<AssetRanking>> GetLayerOneAssetsAsync();
        Task<List<CandlestickExtended>> GetCustomCandlesticksAnalysisAsync(List<CustomCandlestickData> customCandlestickData);
        Task<List<PairExtended>> GetPairsByDataProviderAsync(List<DataProvider> dataProviders, HttpContext? httpContext = null);
        Task<List<PairExtended>> GetPairByIdsAsync(List<long> ids, HttpContext? httpContext = null);
    }
}
