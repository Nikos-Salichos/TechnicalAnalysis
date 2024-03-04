using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Delete;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Delete;

public class DeletePoolsHandler(IPostgreSqlRepository repository) : IRequestHandler<DeletePoolsCommand>
{
    public async Task Handle(DeletePoolsCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteEntitiesByIdsAsync<Pool>(request.Ids, "Pools");
    }
}