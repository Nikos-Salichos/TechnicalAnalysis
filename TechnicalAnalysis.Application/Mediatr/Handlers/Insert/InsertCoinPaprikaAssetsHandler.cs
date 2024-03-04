using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertCoinPaprikaAssetsHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertCoinPaprikaAssetCommand>
    {
        public async Task Handle(InsertCoinPaprikaAssetCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertCoinPaprikaAssetsAsync(request.Assets);
        }
    }
}
