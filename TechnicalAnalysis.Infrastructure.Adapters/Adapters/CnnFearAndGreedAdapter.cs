using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class CnnFearAndGreedAdapter(ICnnStockFearAndGreedHttpClient httpClient, IMediator mediator,
        ILogger<CnnFearAndGreedAdapter> logger) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var cnnProvider = exchanges.Find(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);

            cnnProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            if (cnnProvider.IsProviderAssetPairsSyncedToday())
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return true;
            }

            var response = await httpClient.GetCnnStockFearAndGreedIndex();
            if (response.HasError)
            {
                return false;
            }

            var domainStockFearAndGreed = response.SuccessValue.FearAndGreedHistorical.FearAndGreedHistoricalData.ToDomain();

            var distinctItems = domainStockFearAndGreed.Where(item => item.DateTime.Date != DateTime.UtcNow.Date)
                                                       .GroupBy(item => item.DateTime.Date)
                                                       .Select(group => group.First())
                                                       .ToList();

            await mediator.Send(new InsertStockFearAndGreedIndexCommand(distinctItems));

            cnnProvider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            await mediator.Send(new UpdateExchangeCommand(cnnProvider.ProviderPairAssetSyncInfo));
            return true;
        }
    }
}
