using TechnicalAnalysis.Domain.Contracts.Input.FredApiContracts;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IFredApiClient
    {
        Task<IResult<FredVixContract, string>> SyncVix();
    }
}
