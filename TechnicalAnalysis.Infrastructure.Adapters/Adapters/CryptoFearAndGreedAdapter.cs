using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
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
                logger.LogInformation("Method: {Method} {Provider} synchronized for today", nameof(Sync), provider);
                return;
            }

            var cryptoFearAndGreedIndexData = await mediator.Send(new GetCryptoFearAndGreedIndexQuery());
            int numberOfDates = 10000; // 10000 means all in provider
            if (cryptoFearAndGreedIndexData.Any())
            {
                var latestDataBasedOnDatetime = cryptoFearAndGreedIndexData
                                                             .Select(e => e.TimestampAsDateTime)
                                                             .DefaultIfEmpty(DateTime.MinValue)
                                                             .Max();

                DateTime today = DateTime.Today;

                TimeSpan difference = today - latestDataBasedOnDatetime;
                numberOfDates = difference.Days;
                if (numberOfDates is 0)
                {
                    return;
                }
            }

            var response = await cryptoFearAndGreedHttpClient.GetCryptoFearAndGreedIndex(numberOfDates);

            if (response.IsError)
            {
                return;
            }

            await mediator.Send(new InsertCryptoFearAndGreedIndexCommand(response.SuccessValue));

            alternativeMeCryptoAndFearProvider.UpdateProviderInfo();
            var providerCandlestickSyncInfo = alternativeMeCryptoAndFearProvider.GetOrCreateProviderCandlestickSyncInfo(provider, timeframe);
            await mediator.Send(new UpdateExchangeCommand(alternativeMeCryptoAndFearProvider.ProviderPairAssetSyncInfo, providerCandlestickSyncInfo));
        }
    }
}
