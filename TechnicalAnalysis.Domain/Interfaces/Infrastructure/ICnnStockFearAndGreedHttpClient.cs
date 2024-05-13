using TechnicalAnalysis.Domain.Contracts.Input.Cnn;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICnnStockFearAndGreedHttpClient
    {
        Task<IResult<RootStockFearAndGreed, string>> GetCnnStockFearAndGreedIndex();
    }
}
