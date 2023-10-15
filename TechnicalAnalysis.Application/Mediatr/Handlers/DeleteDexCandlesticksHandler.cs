using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class DeleteDexCandlesticksHandler : IRequestHandler<DeleteDexCandlesticksCommand>
    {
        private readonly IPostgreSqlRepository _repository;

        public DeleteDexCandlesticksHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteDexCandlesticksCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteDexCandlesticksByIdsAsync(request.Ids);
        }
    }
}
