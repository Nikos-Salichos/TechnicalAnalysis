using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertCoinPaprikaAssetsHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertCoinPaprikaAssetCommand>
    {
        public async Task Handle(InsertCoinPaprikaAssetCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertCoinPaprikaAssetsAsync(request.Assets);
        }
    }
}
