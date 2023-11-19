using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers;

public class DeletePoolsHandler(IPostgreSqlRepository repository) : IRequestHandler<DeletePoolsCommand>
{
    public async Task Handle(DeletePoolsCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteEntitiesByIdsAsync<Pool>(request.Ids, "Pools", "Id");
    }
}
