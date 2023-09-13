using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Application.Validators.Binance;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class BinanceAdapter : IAdapter
    {
        private readonly IMediator _mediator;
        private readonly IBinanceHttpClient _binanceHttpClient;
        private readonly ILogger<BinanceAdapter> _logger;

        public BinanceAdapter(IBinanceHttpClient binanceHttpClient, IMediator mediator, ILogger<BinanceAdapter> logger)
        {
            _binanceHttpClient = binanceHttpClient;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Sync(DataProvider provider, Timeframe timeframe)
        {
            var exchanges = await _mediator.Send(new GetPartialProviderQuery());
            var binanceProvider = exchanges.FirstOrDefault(p => p.PrimaryId == (int)provider);

            if (binanceProvider == null)
            {
                _logger.LogWarning("Method {Method}: {Provider} could not be found", nameof(Sync), provider);
                return;
            }

            if (binanceProvider.IsProviderSyncedToday(timeframe))
            {
                _logger.LogInformation("Method: {Method} {Provider} synchronized for today", nameof(Sync), provider);
                return;
            }

            var response = await _binanceHttpClient.GetBinanceAssetsAndPairs();

            if (response.IsError)
            {
                return;
            }

            const string activeStatus = "TRADING";
            var tradeablePairs = response.SuccessValue.Symbols.Where(s => string.Equals(s.Status, activeStatus, StringComparison.InvariantCultureIgnoreCase));

            await SyncAssets(tradeablePairs);
            await SyncPairs(tradeablePairs);

            var fetchedAssetsTask = _mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = _mediator.Send(new GetPairsQuery());
            var fetchedCandlesticksTask = _mediator.Send(new GetCandlesticksQuery());

            await Task.WhenAll(fetchedAssetsTask, fetchedPairsTask, fetchedCandlesticksTask);

            var assets = await fetchedAssetsTask;
            var pairs = (await fetchedPairsTask).Where(fp => fp.Provider == provider).ToList();
            var candlesticks = await fetchedCandlesticksTask;

            pairs.MapPairsToCandlesticks(candlesticks);

            await SyncCandlesticks(pairs.ToContract().ToList(), timeframe);

            binanceProvider.LastAssetSync = DateTime.UtcNow;
            binanceProvider.LastPairSync = DateTime.UtcNow;
            binanceProvider.CandlestickSyncInfos.Add(new ProviderCandlestickSyncInfo
            {
                ProviderId = binanceProvider.PrimaryId,
                TimeframeId = (long)timeframe,
                LastCandlestickSync = DateTime.UtcNow
            });

            await _mediator.Send(new UpdateExchangeCommand(binanceProvider));
        }

        private async Task SyncAssets(IEnumerable<BinanceSymbol> tradeablePairs)
        {
            List<BinanceAsset> newAssets = new();
            foreach (var item in tradeablePairs)
            {
                newAssets.Add(new BinanceAsset { Id = 0, Asset = item.BaseAsset });
                newAssets.Add(new BinanceAsset { Id = 0, Asset = item.QuoteAsset });
            }
            newAssets = newAssets.Distinct(new BinanceAssetComparer()).ToList();

            var fetchedAssets = await _mediator.Send(new GetAssetsQuery());

            var missingAssets = newAssets.Except(fetchedAssets.ToContract(), new BinanceAssetComparer()).Distinct();

            if (missingAssets.Any())
            {
                await _mediator.Send(new InsertAssetsCommand(missingAssets.ToDomain()));
            }
        }

        private async Task SyncPairs(IEnumerable<BinanceSymbol> tradeablePairs)
        {
            var fetchedAssets = (await _mediator.Send(new GetAssetsQuery())).ToList();
            var assetDictionary = fetchedAssets.ToContract().ToImmutableDictionary(asset => asset.Asset, asset => asset.Id);

            var binancePairs = tradeablePairs.Select(tradeablePair => new BinancePair
            {
                Pair = $"{tradeablePair.BaseAsset}-{tradeablePair.QuoteAsset}",
                BaseAssetId = assetDictionary.TryGetValue(tradeablePair.BaseAsset, out long baseAssetId) ? baseAssetId : 0,
                QuoteAssetId = assetDictionary.TryGetValue(tradeablePair.QuoteAsset, out long quoteAssetId) ? quoteAssetId : 0,
                Provider = DataProvider.Binance,
                AllCandles = false,
                IsActive = true,
            });

            binancePairs = PairExtension.GetDollarPairs(fetchedAssets.ToContract(), binancePairs.ToList());
            var fetchedPairs = await _mediator.Send(new GetPairsQuery());
            var newPairs = binancePairs.ToDomain().Where(pair => !fetchedPairs.Contains(pair, new PairExtendedEqualityComparer()));

            foreach (var dollarPair in newPairs)
            {
                dollarPair.IsActive = true;
            }

            if (newPairs.Any())
            {
                await _mediator.Send(new InsertPairsCommand(newPairs));
            }
        }

        private async Task SyncCandlesticks(IList<BinancePair> binancePairs, Timeframe period = Timeframe.Daily)
        {
            var pairsWithExistingCandles = JsonSerializer.Deserialize<IEnumerable<BinancePair>>(JsonSerializer.Serialize(binancePairs));

            foreach (var binancePair in binancePairs)
            {
                var latestCandlestickOpenTime = binancePair.BinanceCandlesticks
                                                             .Select(candlestick => candlestick.OpenTime)
                                                             .DefaultIfEmpty(DateTime.MinValue)
                                                             .Max();

                DateTime startDate = latestCandlestickOpenTime.AddDays(1);
                DateTime endDate = default;

                var yesterday = DateTime.UtcNow.AddDays(-1).Date;

                if (startDate > yesterday.Date)
                {
                    continue;
                }

                if (latestCandlestickOpenTime != default)
                {
                    endDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                }

                if (latestCandlestickOpenTime == default && (period == Timeframe.Daily || period == Timeframe.Weekly))
                {
                    startDate = DateTime.UtcNow.AddYears(-10).AddDays(1).Date;
                    endDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                }
                else if (latestCandlestickOpenTime == default && period == Timeframe.OneHour)
                {
                    startDate = DateTime.UtcNow.AddHours(1).Date.AddHours(DateTime.UtcNow.AddHours(1).Hour).AddYears(-10);
                    endDate = DateTime.Now.Date.AddHours(DateTime.Now.Hour - 1).AddMinutes(59).AddSeconds(59);
                }

                if (binancePairs.Count > 0)
                {
                    string timeframe = "";
                    var dateRanges = new List<(DateTime, DateTime)>();

                    if (period == Timeframe.OneHour)
                    {
                        timeframe = "1h";
                        dateRanges = DatetimeExtension.GetHourlyDateRanges(startDate, endDate);
                    }
                    else if (period == Timeframe.Daily)
                    {
                        timeframe = "1d";
                        dateRanges = DatetimeExtension.GetDailyDateRanges(startDate, endDate);
                    }
                    else if (period == Timeframe.Weekly)
                    {
                        timeframe = "1w";
                        dateRanges = DatetimeExtension.GetWeeklyDateRanges(startDate, endDate);
                    }

                    if (dateRanges.Count == 0)
                    {
                        continue;
                    }

                    const string candlesPerCall = "1000";

                    foreach (var dateRange in dateRanges)
                    {
                        var queryParams = new Dictionary<string, string>
                            {
                                    { "symbol" , binancePair.Pair.Replace("-", "") },
                                    { "interval" , timeframe },
                                    { "startTime", new DateTimeOffset(dateRange.Item1).ToUnixTimeMilliseconds().ToString() },
                                    { "endTime", new DateTimeOffset(dateRange.Item2).ToUnixTimeMilliseconds().ToString() },
                                    { "limit", candlesPerCall },
                            }.ToImmutableDictionary();

                        var response = await _binanceHttpClient.GetBinanceCandlesticks(queryParams);

                        if (response.IsError)
                        {
                            _logger.LogWarning("Method: {Method}: {apiResponse.IsError}", nameof(SyncCandlesticks), response.IsError);
                            continue;
                        }

                        foreach (object[] row in response.SuccessValue)
                        {
                            // Create a new candlestick object.
                            BinanceCandlestick newCandlestick = new BinanceCandlestick();

                            // Get the properties of the candlestick object.
                            PropertyInfo[] properties = typeof(BinanceCandlestick).GetProperties();

                            // Loop through the array elements in the row.
                            int i = 0;
                            foreach (object cell in row)
                            {
                                // Check if i is within the bounds of the properties array.
                                if (i >= properties.Length)
                                {
                                    break;
                                }

                                // Get the corresponding property.
                                PropertyInfo property = properties[i++];

                                if (property.Name == "Volume") // Skip the Volume property
                                {
                                    continue;
                                }

                                // Convert the cell value to the property type and set it on the candlestick object.
                                CandlestickExtension.ParseCandlestickData(newCandlestick, cell, property);
                            }

                            var validator = new BinanceCandlestickValidator();
                            var result = validator.Validate(newCandlestick);

                            bool candlestickExists = binancePair.BinanceCandlesticks.Any(c =>
                                c.OpenTime.EqualsYearMonthDayHourMinute(newCandlestick.OpenTime) &&
                                c.CloseTime.EqualsYearMonthDayHourMinute(newCandlestick.CloseTime));

                            if (result.IsValid && !candlestickExists)
                            {
                                newCandlestick.PairId = binancePair.Id;
                                newCandlestick.Period = timeframe;
                                binancePair.BinanceCandlesticks.Add(newCandlestick);
                            }
                        }
                    }
                }
            }

            if (pairsWithExistingCandles != null)
            {
                binancePairs.FindNewCandlesticks(pairsWithExistingCandles);
            }

            if (binancePairs.Any(pair => pair.BinanceCandlesticks.Count > 0))
            {
                var pairsWithCandlesticks = binancePairs.Where(pair => pair.BinanceCandlesticks.Count > 0);
                var candlesticks = pairsWithCandlesticks.SelectMany(c => c.BinanceCandlesticks);
                await _mediator.Send(new InsertCandlesticksCommand(candlesticks.ToDomain()));
            }
        }
    }
}
