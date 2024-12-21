using TechnicalAnalysis.Domain.Contracts.Input.StockFearAndGreedContracts;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IStockFearAndGreedClient
    {
        Task<Result<StockFearAndGreedRoot, string>> GetStockFearAndGreedIndex();
    }
}
