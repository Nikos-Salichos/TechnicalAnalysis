using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertStockFearAndGreedIndexCommand(List<FearAndGreedModel> stockFearAndGreedData) : IRequest
    {
        public List<FearAndGreedModel> StockFearAndGreedEntities { get; } = stockFearAndGreedData;
    }
}
