using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class DeletePoolsHandler : IRequestHandler<DeletePoolsCommand>
    {
        private readonly IPostgreSqlRepository _repository;

        public DeletePoolsHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeletePoolsCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteEntitiesByIdsAsync<Pool>(request.Ids, "Pools", "Id");
        }
    }
}
