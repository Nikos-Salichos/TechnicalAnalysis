using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertPoolsHandler : IRequestHandler<InsertPoolsCommand>
    {
        private readonly IPostgreSqlRepository _repository;

        public InsertPoolsHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(InsertPoolsCommand request, CancellationToken cancellationToken)
        {
            await _repository.InsertPools(request.Pools);
        }
    }
}
