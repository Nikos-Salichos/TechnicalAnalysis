using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertAssetsHandler : IRequestHandler<InsertAssetsCommand>
    {
        private readonly IPostgreSqlRepository _repository;

        public InsertAssetsHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(InsertAssetsCommand request, CancellationToken cancellationToken)
        {
            await _repository.InsertAssets(request.Assets);
        }
    }
}
