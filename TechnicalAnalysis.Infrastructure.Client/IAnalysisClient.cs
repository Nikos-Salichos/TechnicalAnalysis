using TechnicalAnalysis.CommonModels;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Infrastructure.Client
{
    public interface IAnalysisClient
    {
        Task SynchronizeAsync(Provider provider = Provider.All);
        Task<IEnumerable<PartialPair>> GetPairsIndicatorsAsync(Provider provider = Provider.All);
        Task<IEnumerable<PairExtended>> GetIndicatorsByPairName(string pairName);
    }
}
