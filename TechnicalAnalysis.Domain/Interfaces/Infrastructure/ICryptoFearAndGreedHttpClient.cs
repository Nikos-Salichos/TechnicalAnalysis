using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICryptoFearAndGreedHttpClient
    {
        Task<IResult<List<CryptoFearAndGreedData>, string>> GetCryptoFearAndGreedIndex(int numberOfDates);
    }
}
