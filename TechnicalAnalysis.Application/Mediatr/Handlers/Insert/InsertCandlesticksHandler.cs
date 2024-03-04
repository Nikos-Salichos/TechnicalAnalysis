using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertCandlesticksHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertCandlesticksCommand>
    {
        public async Task Handle(InsertCandlesticksCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertCandlesticksAsync(request.Candlesticks.ToEntity());
        }
    }
}