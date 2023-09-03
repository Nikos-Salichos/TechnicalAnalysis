using TechnicalAnalysis.CommonModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Infrastructure.Client
{
    public interface IAnalysisClient
    {
        Task Synchronize();
        Task<IEnumerable<PartialPair>> GetPairsIndicators(Provider provider = Provider.All);
        Task GetIndicatorsByPairName(string pairName);
    }
}
