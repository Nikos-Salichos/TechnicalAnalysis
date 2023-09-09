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
using PairExtended = TechnicalAnalysis.CommonModels.BusinessModels.PairExtended;
using Provider = TechnicalAnalysis.CommonModels.Enums.Provider;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class AlpacaAdapter : IAdapter
    {
        private readonly ILogger<AlpacaAdapter> _logger;
        private readonly IMediator _mediator;
        private readonly IAlpacaHttpClient _alpacaHttpClient;

        public AlpacaAdapter(ILogger<AlpacaAdapter> logger, IMediator mediator, IAlpacaHttpClient alpacaHttpClient)
        {
            _logger = logger;
            _mediator = mediator;
            _alpacaHttpClient = alpacaHttpClient;
        }

        public async Task Sync(Provider provider)
        {
            var exchanges = await _mediator.Send(new GetExchangesQuery());
            var alpacaProvider = exchanges.FirstOrDefault(p => p.Code == (int)Provider.Alpaca);

            if (alpacaProvider == null)
            {
                _logger.LogWarning("Method {Method}: {Provider} could not be found", nameof(Sync), provider);
                return;
            }

            var stockSymbols = new List<string> { "vt", "vti", "VTV", "PFF", "SPHD", "XLRE", "nke", "ba",
                "tsla", "aapl", "googl", "abnb", "JNJ", "XOM", "WMT", "META", "JPM","V", "KO", "PEP",
                "MCD", "AVGO", "ACN", "NFLX",
                "MA","BAC","MS","WCF","SCHW","RY","MSFT","NVDA","CRM","ABDE","VZ","IBM","EWH","MCHI","EWS",
                "FEZ","IWM","SPY","DIA","DAX","VGK","QQQ",
                "IVV","VUG","VB","VNQ","XLE","XLF","BND","VUG","xom","vea", "VWO", "GLD", "VXUS", "VO", "IWM",
                "XLV", "PYPL","IWD","IJH","ITOT","JEPI","SPYV", "VOT","VDE", "voo", "WBA"
            };

            stockSymbols = stockSymbols.Distinct().ToList();

            var fetchedAssets = await _mediator.Send(new GetAssetsQuery());

            var fetchedAssetNames = fetchedAssets.Select(f => f.Symbol).ToList();

            bool allStockSymbolsExist = true;
            foreach (var symbol in stockSymbols)
            {
                var assetFound = fetchedAssetNames.FirstOrDefault(name => string.Equals(name, symbol, StringComparison.InvariantCultureIgnoreCase));
                if (assetFound is null)
                {
                    allStockSymbolsExist = false;
                    break;
                }
            }

            if (alpacaProvider?.LastAssetSync.Date == DateTime.UtcNow.Date
            && alpacaProvider?.LastPairSync.Date == DateTime.UtcNow.Date
            && alpacaProvider?.LastCandlestickSync.Date == DateTime.UtcNow.Date
            && allStockSymbolsExist)
            {
                _logger.LogInformation("Method: {Method} {Provider} synchronized for today", nameof(Sync), provider);
                return;
            }

            await SyncAssets(fetchedAssets, stockSymbols);
            await SyncPairs(stockSymbols);
            await SyncCandlesticks(Timeframe.Daily);

            alpacaProvider.LastAssetSync = DateTime.UtcNow;
            alpacaProvider.LastPairSync = DateTime.UtcNow;
            alpacaProvider.LastCandlestickSync = DateTime.UtcNow;

            await _mediator.Send(new UpdateExchangeCommand(alpacaProvider));
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
                await _mediator.Send(new InsertAssetsCommand(newAssets));
            }
        }

        public async Task SyncPairs(IEnumerable<string> stockSymbols)
        {
            var fetchedAssetsTask = _mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = _mediator.Send(new GetPairsQuery());
            await Task.WhenAll(fetchedAssetsTask, fetchedPairsTask);

            var assets = (await fetchedAssetsTask).ToList();
            var pairs = (await fetchedPairsTask).Where(fp => fp.Provider == Provider.Alpaca).ToList();

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
                    PairExtended newPair = new PairExtended
                    {
                        BaseAssetId = baseAsset.PrimaryId,
                        BaseAssetName = baseAsset.Symbol,
                        Provider = Provider.Alpaca,
                        Symbol = baseAsset.Symbol,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    newPairs.Add(newPair);
                }
            }

            if (newPairs.Count > 0)
            {
                await _mediator.Send(new InsertPairsCommand(newPairs));
            }
        }

        public async Task SyncCandlesticks(Timeframe period = Timeframe.Daily)
        {
            var fetchedAssetsTask = _mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = _mediator.Send(new GetPairsQuery());
            var fetchedCandlesticksTask = _mediator.Send(new GetCandlesticksQuery());

            await Task.WhenAll(fetchedAssetsTask, fetchedPairsTask, fetchedCandlesticksTask);

            var fetchedAssets = await fetchedAssetsTask;
            var fetchedPairs = await fetchedPairsTask;
            var fetchedCandlesticks = await fetchedCandlesticksTask;

            var pairs = fetchedPairs.Where(p => p.Provider == Provider.Alpaca).ToList();

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
                    toDatetime = DateTime.Now.Date.AddHours(DateTime.Now.Hour - 1).AddMinutes(59).AddSeconds(59);
                }

                var dateRanges = DatetimeExtension.GetDailyDateRanges(fromDatetime, toDatetime);
                var mergedItems = new List<KeyValuePair<string, List<IBar>>>();

                foreach (var dateRange in dateRanges)
                {
                    var stockData = await _alpacaHttpClient.GetAlpacaData(fetchedPair.BaseAssetName, dateRange.Item1, dateRange.Item2, BarTimeFrame.Day);
                    if (stockData.IsError)
                    {
                        _logger.LogWarning("Method: {Method}: {apiResponse.IsError}", nameof(SyncCandlesticks), stockData.IsError);
                        continue;
                    }
                    mergedItems.AddRange(stockData.SuccessValue.Items.Select(kvp => new KeyValuePair<string, List<IBar>>(kvp.Key, kvp.Value.ToList())));
                }

                var mergedData = mergedItems
                    .GroupBy(kvp => kvp.Key)
                    .ToImmutableDictionary(group => group.Key, group => group.SelectMany(kvp => kvp.Value).ToList());

                foreach (var key in mergedData.Keys)
                {
                    foreach (var value in mergedData.Values.SelectMany(v => v))
                    {
                        var builder = new CandlestickBuilder();
                        var candlestick = builder
                               .WithPoolOrPairId(fetchedPair.PrimaryId)
                               .WithPoolOrPairName(key)
                               .WithCloseDate(value.TimeUtc)
                               .WithOpenDate(value.TimeUtc.Date)
                               .WithOpenPrice(value.Open)
                               .WithHighPrice(value.High)
                               .WithLowPrice(value.Low)
                               .WithClosePrice(value.Close)
                               .WithTimeframe(Timeframe.Daily)
                               .Build();

                        newCandlesticks.Add(candlestick);
                    }
                }
            }

            if (newCandlesticks.Count > 0)
            {
                await _mediator.Send(new InsertCandlesticksCommand(newCandlesticks));
            }
        }
    }
}
