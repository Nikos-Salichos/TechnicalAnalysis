using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Delete;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Delete;

public class DeletePoolsHandler(IPostgreSqlRepository repository) : IRequestHandler<DeletePoolsCommand, bool>
{
    public async Task<bool> Handle(DeletePoolsCommand request, CancellationToken cancellationToken)
    {
        var result = await repository.DeleteEntitiesByIdsAsync<Pool>(request.Ids, tableName: "Pools");
        if (result.HasError)
        {
            return false;
        }
        return result.IsSuccess;
    }
}