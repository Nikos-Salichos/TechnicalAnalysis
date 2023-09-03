using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertDexCandlesticksHandler : IRequestHandler<InsertDexCandlesticksCommand>
    {
        private readonly IPostgreSqlRepository _repository;

        public InsertDexCandlesticksHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(InsertDexCandlesticksCommand request, CancellationToken cancellationToken)
        {
            await _repository.InsertCandlesticks(request.DexCandlesticks);
        }
    }
}
