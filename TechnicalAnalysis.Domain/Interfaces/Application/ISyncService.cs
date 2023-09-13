using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Interfaces.Application
{
    public interface ISyncService
    {
        Task<string> SynchronizeProvidersAsync(DataProvider provider, Timeframe timeframe);
    }
}
