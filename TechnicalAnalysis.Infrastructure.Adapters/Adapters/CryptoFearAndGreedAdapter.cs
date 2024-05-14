using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class CryptoFearAndGreedAdapter(ICryptoFearAndGreedHttpClient cryptoFearAndGreedHttpClient, IMediator mediator,
        ILogger<CryptoFearAndGreedAdapter> logger) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var alternativeMeCryptoAndFearProvider = exchanges.Find(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);

            alternativeMeCryptoAndFearProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            if (alternativeMeCryptoAndFearProvider.IsProviderAssetPairsSyncedToday())
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return true;
            }

            var cryptoFearAndGreedIndexData = await mediator.Send(new GetCryptoFearAndGreedIndexQuery());

            int calculatedDates = 0;
            // If there are no elements in cryptoFearAndGreedIndexData we use 0 to fetch data for all available dates
            if (cryptoFearAndGreedIndexData.Count > 0)
            {
                calculatedDates = CalculateNumberOfDates(cryptoFearAndGreedIndexData);
                if (calculatedDates == 0)
                {
                    return true; // If calculated dates are 0 when there are elements, return true
                }
            }

            var response = await cryptoFearAndGreedHttpClient.GetCryptoFearAndGreedIndex(calculatedDates);

            if (response.HasError)
            {
                return false;
            }

            var domainFearAndGreed = response.SuccessValue.ToDomain();

            var distinctItems = domainFearAndGreed.Where(item => item.DateTime.Date != DateTime.UtcNow.Date)
                                                       .GroupBy(item => item.DateTime.Date)
                                                       .Select(group => group.First())
                                                       .ToList();

            await mediator.Send(new InsertCryptoFearAndGreedIndexCommand(distinctItems));

            alternativeMeCryptoAndFearProvider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            await mediator.Send(new UpdateExchangeCommand(alternativeMeCryptoAndFearProvider.ProviderPairAssetSyncInfo));
            return true;
        }

        private static int CalculateNumberOfDates(List<FearAndGreedModel> cryptoFearAndGreedData)
        {
            var latestDataBasedOnDatetime = cryptoFearAndGreedData.Max(e => e.DateTime);
            return (DateTime.Today - latestDataBasedOnDatetime).Days;
        }
    }
}
