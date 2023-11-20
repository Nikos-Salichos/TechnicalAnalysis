using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertDexCandlesticksHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertDexCandlesticksCommand>
    {
        public async Task Handle(InsertDexCandlesticksCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertDexCandlesticksAsync(request.DexCandlesticks);
        }
    }
}
