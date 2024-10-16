using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IDexV3Client
    {
        Task<IResult<DexV3ApiResponse, string>> GetMostActivePoolsAsync(int numberOfPools, int numberOfData, DataProvider provider);
    }
}
