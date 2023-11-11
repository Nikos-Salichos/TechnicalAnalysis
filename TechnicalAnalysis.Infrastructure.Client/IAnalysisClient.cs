using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.JsonOutput;

namespace TechnicalAnalysis.Infrastructure.Client
{
    public interface IAnalysisClient
    {
        Task SynchronizeProvidersAsync(DataProvider provider, Timeframe timeframe);
        Task<IEnumerable<PartialPair>> GetPairsIndicatorsAsync(DataProvider provider);
        Task<IEnumerable<PairExtended>> GetIndicatorsByPairNameAsync(string pairName, Timeframe timeframe);
    }
}
