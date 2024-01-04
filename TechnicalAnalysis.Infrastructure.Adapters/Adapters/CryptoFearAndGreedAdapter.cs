using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mediatr.Commands;
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
        public async Task Sync(DataProvider provider, Timeframe timeframe)
        {
            var exchanges = await mediator.Send(new GetProviderSynchronizationQuery());
            var alternativeMeCryptoAndFearProvider = exchanges.FirstOrDefault(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);

            alternativeMeCryptoAndFearProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            if (alternativeMeCryptoAndFearProvider.IsProviderSyncedToday(timeframe))
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return;
            }

            var cryptoFearAndGreedIndexData = await mediator.Send(new GetCryptoFearAndGreedIndexQuery());

            var calculatedDates = CalculateNumberOfDates(cryptoFearAndGreedIndexData);
            var response = await cryptoFearAndGreedHttpClient.GetCryptoFearAndGreedIndex(calculatedDates);

            if (response.IsError)
            {
                return;
            }

            await mediator.Send(new InsertCryptoFearAndGreedIndexCommand(response.SuccessValue));

            alternativeMeCryptoAndFearProvider.UpdateProviderInfo();
            var providerCandlestickSyncInfo = alternativeMeCryptoAndFearProvider.GetOrCreateProviderCandlestickSyncInfo(provider, timeframe);
            await mediator.Send(new UpdateExchangeCommand(alternativeMeCryptoAndFearProvider.ProviderPairAssetSyncInfo, providerCandlestickSyncInfo));
        }

        private static int CalculateNumberOfDates(IEnumerable<CryptoFearAndGreedData> cryptoFearAndGreedData)
        {
            const int providerAllDaysParam = 1000;

            if (!cryptoFearAndGreedData.Any())
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
