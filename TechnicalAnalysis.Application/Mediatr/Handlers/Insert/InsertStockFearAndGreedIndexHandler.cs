using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertStockFearAndGreedIndexHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertStockFearAndGreedIndexCommand>
    {
        public async Task Handle(InsertStockFearAndGreedIndexCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertStockFearAndGreedIndex(request.StockFearAndGreedEntities);
        }
    }
}
