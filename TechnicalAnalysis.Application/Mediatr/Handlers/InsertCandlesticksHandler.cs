using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertCandlesticksHandler : IRequestHandler<InsertCandlesticksCommand>
    {
        private readonly IPostgreSqlRepository _repository;

        public InsertCandlesticksHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(InsertCandlesticksCommand request, CancellationToken cancellationToken)
        {
            await _repository.InsertCandlesticks(request.Candlesticks.ToEntity());
        }
    }
}