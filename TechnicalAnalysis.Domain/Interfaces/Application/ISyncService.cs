using TechnicalAnalysis.CommonModels.ApiRequests;

namespace TechnicalAnalysis.Domain.Interfaces.Application
{
    public interface ISyncService
    {
        Task SynchronizeProvidersAsync(DataProviderTimeframeRequest dataProviderTimeframeRequest);
    }
}
