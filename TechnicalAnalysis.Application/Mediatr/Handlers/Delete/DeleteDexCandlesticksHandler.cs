using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Delete;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Delete;

public class DeleteDexCandlesticksHandler(IPostgreSqlRepository repository) : IRequestHandler<DeleteDexCandlesticksCommand, bool>
{
    public async Task<bool> Handle(DeleteDexCandlesticksCommand request, CancellationToken cancellationToken)
    {
        var result = await repository.DeleteEntitiesByIdsAsync<DexCandlestick>(request.Ids, tableName: "DexCandlesticks");
        if (result.HasError)
        {
            return false;
        }
        return result.IsSuccess;
    }
}
