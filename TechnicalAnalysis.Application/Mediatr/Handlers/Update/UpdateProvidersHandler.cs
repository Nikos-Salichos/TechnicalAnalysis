using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Update
{
    public class UpdateProvidersHandler(IPostgreSqlRepository repository) : IRequestHandler<UpdateExchangeCommand>
    {
        public async Task Handle(UpdateExchangeCommand request, CancellationToken cancellationToken)
        {
            if (request.ProviderPairAssetSyncInfo is not null)
            {
                await repository.UpdateProviderPairAssetSyncInfoAsync(request.ProviderPairAssetSyncInfo);
            }

            if (request.ProviderCandlestickSyncInfo is not null)
            {
                await repository.UpdateProviderCandlestickSyncInfoAsync(request.ProviderCandlestickSyncInfo);
            }
        }
    }
}
