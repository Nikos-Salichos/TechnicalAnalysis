using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoAndFearIndex;
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

            var cryptoFearAndGreedIndexData = (await mediator.Send(new GetCryptoFearAndGreedIndexQuery())).ToList();

            var calculatedDates = CalculateNumberOfDates(cryptoFearAndGreedIndexData);
            var response = await cryptoFearAndGreedHttpClient.GetCryptoFearAndGreedIndex(calculatedDates);

            if (response.HasError)
            {
                return false;
            }

            await mediator.Send(new InsertCryptoFearAndGreedIndexCommand(response.SuccessValue));

            alternativeMeCryptoAndFearProvider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            await mediator.Send(new UpdateExchangeCommand(alternativeMeCryptoAndFearProvider.ProviderPairAssetSyncInfo));
            return true;
        }

        private static int CalculateNumberOfDates(List<CryptoFearAndGreedData> cryptoFearAndGreedData)
        {
            const int providerAllDaysParam = 1000;

            if (cryptoFearAndGreedData.Count == 0)
            {
                return providerAllDaysParam;
            }

            var latestDataBasedOnDatetime = cryptoFearAndGreedData.Max(e => e.TimestampAsDateTime);
            int numberOfDates = (DateTime.Today - latestDataBasedOnDatetime).Days;

            return numberOfDates is not 0
                ? numberOfDates
                : providerAllDaysParam;
        }
    }
}
