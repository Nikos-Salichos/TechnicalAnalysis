using TechnicalAnalysis.Domain.Contracts.Input.Cnn;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface ICnnStockFearAndGreedClient
    {
        Task<Result<RootStockFearAndGreed, string>> GetCnnStockFearAndGreedIndex();
    }
}
