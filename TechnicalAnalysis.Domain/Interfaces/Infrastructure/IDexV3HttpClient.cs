using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IDexV3HttpClient
    {
        Task<DexV3ApiResponse> GetMostActivePools(int numberOfPools, int numberOfData, Provider provider);
    }
}
