using TechnicalAnalysis.Domain.Contracts.Input.FredApiContracts;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IFredApiClient
    {
        Task<Result<FredVixContract, string>> SyncVix();
    }
}
