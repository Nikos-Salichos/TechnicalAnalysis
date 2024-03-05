using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertAssetsRankingHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertAssetsRankingCommand>
    {
        public async Task Handle(InsertAssetsRankingCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertCoinPaprikaAssetsAsync(request.Assets);
        }
    }
}
