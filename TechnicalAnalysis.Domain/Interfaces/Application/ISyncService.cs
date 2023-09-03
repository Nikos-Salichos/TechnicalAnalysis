using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Interfaces.Application
{
    public interface ISyncService
    {
        Task SynchronizeProvidersAsync(Provider provider);
    }
}
