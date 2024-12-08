using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICryptoFearAndGreedClient
    {
        Task<Result<List<CryptoFearAndGreedData>, string>> GetCryptoFearAndGreedIndex(int numberOfDates);
    }
}
