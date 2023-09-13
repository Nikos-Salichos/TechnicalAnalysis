using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.JsonOutput;

namespace TechnicalAnalysis.Infrastructure.Client
{
    public interface IAnalysisClient
    {
        Task SynchronizeAsync(DataProvider provider = DataProvider.All);
        Task<IEnumerable<PartialPair>> GetPairsIndicatorsAsync(DataProvider provider = DataProvider.All);
        Task<IEnumerable<PairExtended>> GetIndicatorsByPairName(string pairName);
    }
}
