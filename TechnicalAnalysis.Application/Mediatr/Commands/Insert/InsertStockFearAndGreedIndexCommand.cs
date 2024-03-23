using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertStockFearAndGreedIndexCommand(StockFearAndGreedDomain stockFearAndGreedDatas) : IRequest
    {
        public StockFearAndGreedDomain StockFearAndGreedEntities { get; } = stockFearAndGreedDatas;
    }
}
