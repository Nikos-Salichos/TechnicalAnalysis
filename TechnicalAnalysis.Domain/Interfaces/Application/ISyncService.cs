using TechnicalAnalysis.CommonModels.ApiRequests;

namespace TechnicalAnalysis.Domain.Interfaces.Application
{
    public interface ISyncService
    {
        Task<string> SynchronizeProvidersAsync(DataProviderTimeframeRequest dataProviderTimeframeRequest);
    }
}
