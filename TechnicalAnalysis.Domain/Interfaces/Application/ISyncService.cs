using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Interfaces.Application
{
    public interface ISyncService
    {
        Task<string> SynchronizeProvidersAsync(Provider provider, Timeframe timeframe);
    }
}
