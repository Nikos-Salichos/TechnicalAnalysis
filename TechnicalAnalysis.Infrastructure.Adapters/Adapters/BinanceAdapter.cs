using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.Application.Validations.Binance;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class BinanceAdapter(IBinanceHttpClient binanceHttpClient, IMediator mediator, ILogger<BinanceAdapter> logger) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var binanceProvider = exchanges.Find(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);

            binanceProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            if (binanceProvider.IsProviderAssetPairsSyncedToday() && binanceProvider.IsProviderCandlesticksSyncedToday(timeframe))
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return true;
            }

            var response = await binanceHttpClient.GetBinanceAssetsAndPairs();

            if (response.HasError)
            {
                return false;
            }

            const string activeStatus = "TRADING";
            var tradeableBinancePairs = response.SuccessValue.Symbols.Where(s => string.Equals(s.Status, activeStatus, StringComparison.InvariantCultureIgnoreCase)).ToList();

            await SyncAssets(tradeableBinancePairs);
            await SyncPairs(tradeableBinancePairs);

            var fetchedPairsTask = mediator.Send(new GetPairsQuery());
            var fetchedCandlesticksTask = mediator.Send(new GetCandlesticksQuery());

            var pairs = (await fetchedPairsTask).Where(fp => fp.Provider == provider).ToList();
            var candlesticks = await fetchedCandlesticksTask;

            pairs.MapPairsToCandlesticks(candlesticks);

            await SyncCandlesticks(pairs.ToContract(), timeframe);

            binanceProvider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            var providerCandlestickSyncInfo = binanceProvider.GetOrCreateProviderCandlestickSyncInfo(provider, timeframe);
            await mediator.Send(new UpdateExchangeCommand(binanceProvider.ProviderPairAssetSyncInfo, providerCandlestickSyncInfo));
            return true;
        }

        private async Task SyncAssets(List<BinanceSymbol> tradeablePairs)
        {
            List<BinanceAsset> newAssets = [];
            foreach (var item in tradeablePairs)
            {
                newAssets.Add(new BinanceAsset { Id = 0, Asset = item.BaseAsset });
                newAssets.Add(new BinanceAsset { Id = 0, Asset = item.QuoteAsset });
            }
            newAssets = newAssets.Distinct().ToList();

            var fetchedAssets = await mediator.Send(new GetAssetsQuery());
            var missingAssets = newAssets.ToDomain().Except(fetchedAssets).Distinct().ToList();

            if (missingAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsCommand(missingAssets));
            }
        }

        private async Task SyncPairs(List<BinanceSymbol> tradeableBinancePairs)
        {
            var fetchedPairsTask = mediator.Send(new GetPairsQuery());
            var fetchedAssets = await mediator.Send(new GetAssetsQuery());
            var assetDictionary = fetchedAssets
                .Where(asset => asset.Symbol != null)
                .ToDictionary(asset => asset.Symbol!, asset => asset.PrimaryId);

            var binancePairs = tradeableBinancePairs.ConvertAll(tradeablePair => new PairExtended
            {
                Symbol = $"{tradeablePair.BaseAsset}-{tradeablePair.QuoteAsset}",
                BaseAssetId = assetDictionary.TryGetValue(tradeablePair.BaseAsset, out long baseAssetId) ? baseAssetId : 0,
                QuoteAssetId = assetDictionary.TryGetValue(tradeablePair.QuoteAsset, out long quoteAssetId) ? quoteAssetId : 0,
                Provider = DataProvider.Binance,
                AllCandles = false,
                IsActive = true,
            });

            var newDollarPairs = PairExtension.GetUniqueDollarPairs(fetchedAssets, binancePairs);
            var fetchedPairs = await fetchedPairsTask;

            var missingDollarPairs = newDollarPairs.Except(fetchedPairs).Distinct().ToList();
            foreach (var dollarPair in missingDollarPairs)
            {
                dollarPair.IsActive = true;
            }

            if (missingDollarPairs.Count > 0)
            {
                await mediator.Send(new InsertPairsCommand(missingDollarPairs));
            }
        }

        private async Task SyncCandlesticks(List<BinancePair> binancePairs, Timeframe period = Timeframe.Daily)
        {
            var pairsWithExistingCandles = JsonSerializer.Deserialize<List<BinancePair>>(JsonSerializer.Serialize(binancePairs));

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
                    endDate = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour - 1).AddMinutes(59).AddSeconds(59);
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
                            }.ToDictionary();

                        var response = await binanceHttpClient.GetBinanceCandlesticks(queryParams);

                        if (response.HasError)
                        {
                            logger.LogError("Error: {FailValue}, queryParams: {@QueryParams}", response.FailValue, queryParams);
                            break;
                        }

                        foreach (object[] row in response.SuccessValue)
                        {
                            // Create a new candlestick object.
                            BinanceCandlestick newCandlestick = new();

                            // Get the properties of the candlestick object.
                            PropertyInfo[] properties = typeof(BinanceCandlestick).GetProperties();

                            // Loop through the array elements in the row.
                            int i = 0;
                            foreach (var cell in row)
                            {
                                // Check if i is within the bounds of the properties array.
                                if (i >= properties.Length)
                                {
                                    break;
                                }

                                // Get the corresponding property.
                                PropertyInfo property = properties[i++];

                                // Convert the cell value to the property type and set it on the candlestick object.
                                CandlestickExtension.ParseCandlestickData(newCandlestick, cell, property);
                            }

                            var validator = new BinanceCandlestickValidator();
                            var result = validator.Validate(newCandlestick);

                            bool candlestickExists = binancePair.BinanceCandlesticks.Exists(c =>
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

            if (binancePairs.Exists(pair => pair.BinanceCandlesticks.Count > 0))
            {
                var uniqueCandlesticks = new HashSet<BinanceCandlestick>(binancePairs
                    .SelectMany(pair => pair.BinanceCandlesticks)
                    .Where(candlestick => candlestick != null))
                    .ToList();

                await mediator.Send(new InsertCandlesticksCommand(uniqueCandlesticks.ToDomain()));
            }
        }
    }
}
