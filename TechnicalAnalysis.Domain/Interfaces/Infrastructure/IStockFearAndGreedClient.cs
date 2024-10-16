using TechnicalAnalysis.Domain.Contracts.Input.StockFearAndGreedContracts;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IStockFearAndGreedClient
    {
        Task<IResult<StockFearAndGreedRoot, string>> GetStockFearAndGreedIndex();
    }
}
