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
    public class CryptoFearAndGreedAdapter(ICryptoFearAndGreedClient cryptoFearAndGreedHttpClient, IMediator mediator,
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

            // Step 1: Get existing dates from cryptoFearAndGreedIndexData
            var existingDates = cryptoFearAndGreedIndexData
                .Select(item => item.DateTime.Date)
                .Distinct()
                .ToHashSet();

            // Step 2: Filter distinct items ensuring uniqueness and exclude today's data
            var distinctItems = domainFearAndGreed
                .Where(item => item.DateTime.Date != DateTime.UtcNow.Date)  // Exclude today's date
                .GroupBy(item => item.DateTime.Date)
                .Select(group => group.First())
                .Where(item => !existingDates.Contains(item.DateTime.Date)) // Exclude dates that already exist in DB data
                .ToList();

            // Insert only unique items
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
