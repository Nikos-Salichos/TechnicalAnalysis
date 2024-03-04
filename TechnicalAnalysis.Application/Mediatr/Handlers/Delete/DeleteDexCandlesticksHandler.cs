using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Delete;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Delete
{
    public class DeleteDexCandlesticksHandler(IPostgreSqlRepository repository) : IRequestHandler<DeleteDexCandlesticksCommand>
    {
        public async Task Handle(DeleteDexCandlesticksCommand request, CancellationToken cancellationToken)
        {
            await repository.DeleteEntitiesByIdsAsync<DexCandlestick>(request.Ids, "DexCandlesticks");
        }
    }
}
