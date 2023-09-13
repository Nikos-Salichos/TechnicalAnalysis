using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Interfaces.Application
{
    public interface IAnalysisService
    {
        Task<IEnumerable<PairExtended>> GetPairsIndicatorsAsync(DataProvider provider = DataProvider.All);
        Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(string pairName);
    }
}
