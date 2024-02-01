using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertCandlesticksHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertCandlesticksCommand>
    {
        public async Task Handle(InsertCandlesticksCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertCandlesticksAsync(request.Candlesticks.ToEntity());
        }
    }
}