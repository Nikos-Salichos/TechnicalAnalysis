using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertDexCandlesticksHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertDexCandlesticksCommand>
    {
        public async Task Handle(InsertDexCandlesticksCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertDexCandlesticksAsync(request.DexCandlesticks);
        }
    }
}
