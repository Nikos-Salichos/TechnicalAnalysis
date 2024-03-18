using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IAdapter
    {
        Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges);
    }
}