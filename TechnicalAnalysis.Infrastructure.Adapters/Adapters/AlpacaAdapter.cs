using Alpaca.Markets;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Builders;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using Asset = TechnicalAnalysis.CommonModels.BusinessModels.Asset;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;
using PairExtended = TechnicalAnalysis.CommonModels.BusinessModels.PairExtended;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class AlpacaAdapter(ILogger<AlpacaAdapter> logger, IMediator mediator, IAlpacaHttpClient alpacaHttpClient) : IAdapter
    {
        public async Task Sync(DataProvider provider, Timeframe timeframe)
        {
            var exchanges = await mediator.Send(new GetProviderSynchronizationQuery());
            var alpacaProvider = exchanges.FirstOrDefault(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);

            alpacaProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            var stockSymbols = new List<string> {
                "vt","vti", "VTV", "PFF", "SPHD", "XLRE", "nke", "ba",
                "tsla", "aapl", "googl", "abnb", "JNJ", "XOM","xom", "WMT", "META", "JPM","V", "KO", "PEP",
                "MCD", "AVGO", "ACN", "NFLX",
                "MA","BAC","MS","WCF","SCHW","RY","MSFT","NVDA","CRM","ABDE","VZ","IBM","EWH","MCHI","EWS",
                "FEZ","IWM","SPY","DIA","DAX","VGK","QQQ",
                "IVV","VUG","VB","VNQ","XLE","XLF","BND","VUG","vea", "VWO", "GLD", "VXUS", "VO", "IWM",
                "XLV", "PYPL","IWD","IJH","ITOT","JEPI","SPYV", "VOT","VDE", "voo", "WBA",
                "BRK.A",
                "AMZN"
            };

            stockSymbols = stockSymbols.Select(symbol => symbol.ToUpper())
                                       .Distinct(StringComparer.InvariantCultureIgnoreCase)
                                       .ToList();

            var fetchedAssets = await mediator.Send(new GetAssetsQuery());

            var fetchedAssetNames = fetchedAssets.Select(f => f.Symbol).ToList();

            bool allStockSymbolsExist = true;
            foreach (var symbol in stockSymbols)
            {
                var assetFound = fetchedAssetNames.Find(name => string.Equals(name, symbol, StringComparison.InvariantCultureIgnoreCase));
                if (assetFound is null)
                {
                    allStockSymbolsExist = false;
                    break;
                }
            }

            if (alpacaProvider.IsProviderSyncedToday(timeframe) && allStockSymbolsExist)
            {
                logger.LogInformation("Method: {Method} {Provider} synchronized for today", nameof(Sync), provider);
                return;
            }

            await SyncAssets(fetchedAssets, stockSymbols);
            await SyncPairs(stockSymbols);
            await SyncCandlesticks(timeframe);

            alpacaProvider.UpdateProviderInfo();
            var providerCandlestickSyncInfo = alpacaProvider.GetOrCreateProviderCandlestickSyncInfo(provider, timeframe);
            await mediator.Send(new UpdateExchangeCommand(alpacaProvider.ProviderPairAssetSyncInfo, providerCandlestickSyncInfo));
        }

        private async Task SyncAssets(IEnumerable<Asset> fetchedAssets, IEnumerable<string> stockSymbols)
        {
            var existingAssetNames = fetchedAssets.Select(a => a.Symbol).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            List<Asset> newAssets = new List<Asset>();
            foreach (var stockSymbol in stockSymbols)
            {
                if (!existingAssetNames.Contains(stockSymbol, StringComparer.InvariantCultureIgnoreCase))
                {
                    Asset newAsset = new() { Symbol = stockSymbol };
                    newAssets.Add(newAsset);
                }
            }

            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsCommand(newAssets));
            }
        }

        public async Task SyncPairs(IEnumerable<string> stockSymbols)
        {
            var fetchedAssetsTask = mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = mediator.Send(new GetPairsQuery());
            await Task.WhenAll(fetchedAssetsTask, fetchedPairsTask);

            var assets = (await fetchedAssetsTask).ToList();
            var pairs = (await fetchedPairsTask).Where(fp => fp.Provider == DataProvider.Alpaca).ToList();

            var newPairs = new List<PairExtended>();
            foreach (var stockSymbol in stockSymbols)
            {
                var baseAsset = assets.Find(a => string.Equals(a.Symbol, stockSymbol, StringComparison.OrdinalIgnoreCase));

                if (baseAsset is null)
                {
                    return;
                }

                var pairExists = pairs.Find(fp => fp.BaseAssetId == baseAsset?.PrimaryId);

                if (pairExists is null)
                {
                    PairExtended newPair = new()
                    {
                        BaseAssetId = baseAsset.PrimaryId,
                        BaseAssetName = baseAsset.Symbol,
                        Provider = DataProvider.Alpaca,
                        Symbol = baseAsset.Symbol,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    newPairs.Add(newPair);
                }
            }

            if (newPairs.Count > 0)
            {
                await mediator.Send(new InsertPairsCommand(newPairs));
            }
        }

        public async Task SyncCandlesticks(Timeframe period = Timeframe.Daily)
        {
            var fetchedAssetsTask = mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = mediator.Send(new GetPairsQuery());
            var fetchedCandlesticksTask = mediator.Send(new GetCandlesticksQuery());

            await Task.WhenAll(fetchedAssetsTask, fetchedPairsTask, fetchedCandlesticksTask);

            var fetchedAssets = await fetchedAssetsTask;
            var fetchedPairs = await fetchedPairsTask;
            var fetchedCandlesticks = await fetchedCandlesticksTask;

            var pairs = fetchedPairs.Where(p => p.Provider == DataProvider.Alpaca).ToList();

            pairs.MapPairsToAssets(fetchedAssets);
            pairs.MapPairsToCandlesticks(fetchedCandlesticks);

            var newCandlesticks = new List<CandlestickExtended>();

            foreach (var fetchedPair in pairs)
            {
                var latestCandlestickOpenTime = fetchedPair.Candlesticks
                                             .Select(candlestick => candlestick.OpenDate)
                                             .DefaultIfEmpty(DateTime.MinValue)
                                             .Max();

                DateTime fromDatetime = latestCandlestickOpenTime.AddDays(1);
                DateTime toDatetime = default;

                var yesterday = DateTime.UtcNow.AddDays(-1).Date;

                if (fromDatetime > yesterday.Date)
                {
                    continue;
                }

                if (latestCandlestickOpenTime != default)
                {
                    toDatetime = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                }

                if (latestCandlestickOpenTime == default && (period == Timeframe.Daily || period == Timeframe.Weekly))
                {
                    fromDatetime = DateTime.UtcNow.AddYears(-10).AddDays(1).Date;
                    toDatetime = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                }
                else if (latestCandlestickOpenTime == default && period == Timeframe.OneHour)
                {
                    fromDatetime = DateTime.UtcNow.AddHours(1).Date.AddHours(DateTime.UtcNow.AddHours(1).Hour).AddYears(-10);
                    toDatetime = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour - 1).AddMinutes(59).AddSeconds(59);
                }

                foreach (var dateRange in DatetimeExtension.GetDailyDateRanges(fromDatetime, toDatetime))
                {
                    var stockData = await alpacaHttpClient.GetAlpacaData(fetchedPair.BaseAssetName, dateRange.Item1, dateRange.Item2, BarTimeFrame.Day);
                    if (stockData.IsError)
                    {
                        logger.LogWarning("Method: {Method}: {ResponseError}", nameof(SyncCandlesticks), stockData.IsError);
                        continue;
                    }

                    foreach (var kvp in stockData.SuccessValue.Items)
                    {
                        foreach (var bar in kvp.Value)
                        {
                            var candlestick = new CandlestickBuilder()
                                .WithPoolOrPairId(fetchedPair.PrimaryId)
                                .WithPoolOrPairName(kvp.Key)
                                .WithCloseDate(bar.TimeUtc)
                                .WithOpenDate(bar.TimeUtc.Date)
                                .WithOpenPrice(bar.Open)
                                .WithHighPrice(bar.High)
                                .WithLowPrice(bar.Low)
                                .WithClosePrice(bar.Close)
                                .WithTimeframe(Timeframe.Daily)
                                .Build();

                            newCandlesticks.Add(candlestick);
                        }
                    }
                }
            }

            if (newCandlesticks.Count > 0)
            {
                await mediator.Send(new InsertCandlesticksCommand(newCandlesticks));
            }
        }
    }
}
