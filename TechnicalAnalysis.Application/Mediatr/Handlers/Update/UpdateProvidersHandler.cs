﻿using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Update
{
    public class UpdateProvidersHandler(IPostgreSqlRepository repository) : IRequestHandler<UpdateExchangeCommand>
    {
        public async Task Handle(UpdateExchangeCommand request, CancellationToken cancellationToken)
        {
            await repository.UpdateProviderPairAssetSyncInfoAsync(request.ProviderPairAssetSyncInfo);
            await repository.UpdateProviderCandlestickSyncInfoAsync(request.ProviderCandlestickSyncInfo);
        }
    }
}