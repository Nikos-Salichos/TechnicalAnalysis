using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.JsonOutput;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoAndFearIndex;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Utilities;
using Indicator = TechnicalAnalysis.CommonModels.JsonOutput.Indicator;

namespace TechnicalAnalysis.Application.Services
{
    public class AnalysisService(ILogger<AnalysisService> logger, IMediator mediator, IConfiguration configuration)
        : IAnalysisService
    {
        public async Task<IEnumerable<PairExtended>> GetPairsIndicatorsAsync(DataProvider provider, HttpContext? httpContext = null)
        {
            var pairs = await FormatAssetsPairsCandlesticks();

            pairs = provider switch
            {
                DataProvider.Binance => pairs.Where(p => p.Provider == DataProvider.Binance).ToList(),
                DataProvider.Uniswap => pairs.Where(p => p.Provider == DataProvider.Uniswap).ToList(),
                DataProvider.Pancakeswap => pairs.Where(p => p.Provider == DataProvider.Pancakeswap).ToList(),
                DataProvider.Alpaca => pairs.Where(p => p.Provider == DataProvider.Alpaca).ToList(),
                _ => pairs
            };

            if (pairs.Count is 0)
            {
                return Enumerable.Empty<PairExtended>();
            }

            await CalculateTechnicalIndicators(pairs);

            var filteredPairs = FilterPairs(pairs, c => c.EnhancedScans.Count > 0);

            return filteredPairs;
        }

        private static IEnumerable<PairExtended> FilterPairs(IEnumerable<PairExtended> pairs, Func<CandlestickExtended, bool> predicate)
        {
            return pairs.OrderByDescending(pair => pair.CreatedAt)
                        .Select(pair =>
                        {
                            pair.Candlesticks = pair.Candlesticks
                                                .Where(predicate)
                                                .OrderByDescending(c => c.CloseDate)
                                                .GroupBy(c => c.PoolOrPairId)
                                                .Select(group => group.First()) // Take the first item of each group
                                                .ToList();
                            return pair;
                        })
                        .Where(pair => pair.Candlesticks.Count > 0);
        }

        public async Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(IEnumerable<string> pairNames, Timeframe timeframe)
        {
            var fetchedPairs = await FormatAssetsPairsCandlesticks();
            var selectedPairs = fetchedPairs.Where(p => pairNames.Contains(p.Symbol, StringComparer.InvariantCultureIgnoreCase));

            if (!selectedPairs.Any())
            {
                return Enumerable.Empty<PairExtended>();
            }

            await CalculateTechnicalIndicators(fetchedPairs);

            var indicatorReportsPerPair = new Dictionary<PairExtended, List<Indicator>>();

            foreach (var selectedPair in selectedPairs)
            {
                selectedPair.Candlesticks = selectedPair.Candlesticks.OrderBy(c => c.OpenDate).ToList();

                var positionsCloseOneByOne = selectedPair.AverageDownStrategyCloseOneByOnBasedInFractalBreak();
                var positionsCloseAll = selectedPair.AverageDownStrategyCloseAllBasedInFractalBreak();

                var indicatorReports = new List<Indicator>
                {
                    CalculateEnhancedScanSignal(positionsCloseOneByOne, "EnhancedScan_CloseOneByOne", selectedPair.Symbol),
                    CalculateEnhancedScanSignal(positionsCloseAll, "EnhancedScan_CloseAll", selectedPair.Symbol),
                    CalculateEnhancedScanAllSignals(selectedPair),
                    PrintFractalTrend(selectedPair),
                    PrintLowestHighSignals(selectedPair),
                    PrintFlagNestedBodySignals(selectedPair),
                    PrintFlagNestedRangeSignals(selectedPair),
                    PrintFunnelSignals(selectedPair),
                    PrintResistanceBreakoutSignals(selectedPair),
                    CalculateCandlestickCloseBelowPivotPrice(selectedPair)
                };

                indicatorReportsPerPair.Add(selectedPair, indicatorReports);
            }

            // Now indicatorReportsPerPair contains the generated indicators for each selected pair.


            var baseDirectory = GetBaseDirectory();
            foreach (var indicatorReportPerPair in indicatorReportsPerPair)
            {
                foreach (var indicatorReport in indicatorReportPerPair.Value)
                {
                    string candlestickFileName = Path.Combine(baseDirectory, $"{indicatorReportPerPair.Key.Symbol}-candlesticks.json");
                    await JsonHelper.SerializeToJson(indicatorReportPerPair.Key.ToOutputContract(), candlestickFileName);
                    string signalFileName = Path.Combine(baseDirectory, $"{indicatorReportPerPair.Key.Symbol}-{indicatorReport.Name}.json");
                    await JsonHelper.SerializeToJsonArray(indicatorReport, signalFileName);
                }
            }

            return selectedPairs;
        }

        private string GetBaseDirectory()
        {
            var baseDirectory = configuration.GetSection("OutputFolder:Path").Value;

            return string.IsNullOrWhiteSpace(baseDirectory)
                ? string.Empty
                : baseDirectory;
        }

        private static Indicator CalculateEnhancedScanSignal(IEnumerable<Position> positionsStrategy, string indicatorName, string pairSymbol)
        {
            var enhancedScan = new Indicator { Name = indicatorName, PairName = pairSymbol };
            foreach (var position in positionsStrategy)
            {
                var openPosition = new Signal
                {
                    OpenedAt = position.OpenPositionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Buy = 1,
                };
                enhancedScan.Signals.Add(openPosition);

                if (position.ClosePositionDate != default)
                {
                    var closePosition = new Signal
                    {
                        OpenedAt = position.ClosePositionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Sell = 1,
                    };
                    enhancedScan.Signals.Add(closePosition);
                }
            }

            return enhancedScan;
        }

        private static Indicator CalculateCandlestickCloseBelowPivotPrice(PairExtended pair)
        {
            var closeBelowPivotPrice = new Indicator { Name = "closeBelowPivotPrice" };

            foreach (var candlestick in pair.Candlesticks.Where(c => c.CloseRelativeToPivots.FirstOrDefault()?.NumberOfConsecutiveCandlestickBelowPivot >= 3))
            {
                var signalIndicator = new Signal
                {
                    OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Buy = 1,
                };
                closeBelowPivotPrice.Signals.Add(signalIndicator);
            }

            return closeBelowPivotPrice;
        }

        private static Indicator CalculateEnhancedScanAllSignals(PairExtended pair)
        {
            var enhancedScan = new Indicator { Name = "EnhancedScanAllSignals", PairName = pair.Symbol };

            foreach (var candlestick in pair.Candlesticks
                .Where(c => c.EnhancedScans?.FirstOrDefault()?.OrderOfSignal >= 0))
            {
                var signalIndicator = new Signal
                {
                    OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Buy = 1,
                };
                enhancedScan.Signals.Add(signalIndicator);
            }
            return enhancedScan;
        }

        private static Indicator PrintFractalTrend(PairExtended pair)
        {
            var fractalTrend = new Indicator { Name = "FractalTrend", PairName = pair.Symbol };

            for (int i = 0; i < pair?.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = i - 1 >= 0 ? pair.Candlesticks[i - 1] : null;

                if (candlestick1 is null)
                {
                    continue;
                }

                if ((fractalTrend.Signals.Count > 0 && fractalTrend.Signals[fractalTrend.Signals.Count - 1].Sell == 1)
                    || fractalTrend.Signals.Count == 0)
                {
                    if ((candlestick.PriceTrend is Trend.Up && candlestick1.PriceTrend is not Trend.Up)
                        || (candlestick.FractalTrend is Trend.Up && candlestick1.FractalTrend is not Trend.Up))
                    {
                        var signalIndicator = new Signal
                        {
                            OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Buy = 1,
                        };
                        fractalTrend.Signals.Add(signalIndicator);
                    }
                }

                if ((fractalTrend.Signals.Count > 0 && fractalTrend.Signals[fractalTrend.Signals.Count - 1].Buy == 1)
                    || fractalTrend.Signals.Count == 0)
                {
                    if ((candlestick.PriceTrend is Trend.Down && candlestick1.PriceTrend is not Trend.Down)
                    || (candlestick.FractalTrend is Trend.Down && candlestick1.FractalTrend is not Trend.Down))
                    {
                        var signalIndicator = new Signal
                        {
                            OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Sell = 1,
                        };
                        fractalTrend.Signals.Add(signalIndicator);
                    }
                }
            }

            return fractalTrend;
        }

        private static Indicator PrintLowestHighSignals(PairExtended pair)
        {
            var lowestHighLowestLowFractal = new Indicator { Name = "lowestHighLowestLowFractal", PairName = pair.Symbol };

            var lastSignal = string.Empty;
            for (int i = 0; i < pair?.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = i - 1 >= 0 ? pair.Candlesticks[i - 1] : null;

                if (candlestick1 == null)
                {
                    continue;
                }

                if (candlestick1.FractalTrend == Trend.Up && candlestick.ClosePrice > candlestick1.HighPrice
                    && (string.IsNullOrWhiteSpace(lastSignal) || string.Equals(lastSignal, "sell", StringComparison.InvariantCultureIgnoreCase)))
                {
                    lastSignal = "buy";
                    var signalIndicator = new Signal
                    {
                        OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Buy = 1,
                    };
                    lowestHighLowestLowFractal.Signals.Add(signalIndicator);
                }

                if (candlestick1.FractalTrend == Trend.Down && candlestick.ClosePrice < candlestick1.LowPrice
                    && (string.IsNullOrWhiteSpace(lastSignal) || string.Equals(lastSignal, "buy", StringComparison.InvariantCultureIgnoreCase)))
                {
                    lastSignal = "sell";
                    var signalIndicator = new Signal
                    {
                        OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Sell = 1,
                    };
                    lowestHighLowestLowFractal.Signals.Add(signalIndicator);
                }
            }

            return lowestHighLowestLowFractal;
        }

        private static Indicator PrintFlagNestedBodySignals(PairExtended pair)
        {
            var indicator = new Indicator { Name = "FlagNestedBodySignals", PairName = pair.Symbol };

            if (pair.Candlesticks.Count == 0)
            {
                return indicator;
            }

            foreach (var candlestick in pair.Candlesticks)
            {
                foreach (var signal in candlestick.FlagsNestedCandlesticksBody)
                {
                    if (signal.NumberOfNestedCandlestickBodies >= 3 &&
                        (candlestick.FractalTrend is Trend.Down || candlestick.PriceTrend is Trend.Down))
                    {
                        var candlestickFlagPoleId = pair?.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);
                        if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue)
                        {
                            var signalFound = indicator.Signals.FirstOrDefault(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));

                            if (signalFound is not null)
                            {
                                continue;
                            }

                            var signalIndicator = new Signal
                            {
                                OpenedAt = candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                Buy = 1,
                                EntryPrice = candlestick.ClosePrice
                            };

                            indicator.Signals.Add(signalIndicator);
                        }
                    }
                }

                foreach (var signal in candlestick.FlagsNestedCandlesticksRange)
                {
                    if (signal.NumberOfNestedCandlestickRanges > 3 &&
                        (candlestick.FractalTrend is Trend.Down || candlestick.PriceTrend is Trend.Down))
                    {
                        var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);
                        if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue)
                        {
                            var signalFound = indicator.Signals.FirstOrDefault(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));

                            if (signalFound is not null)
                            {
                                continue;
                            }

                            var signalIndicator = new Signal
                            {
                                OpenedAt = candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                Buy = 1,
                                EntryPrice = candlestick.ClosePrice
                            };

                            indicator.Signals.Add(signalIndicator);
                        }
                    }
                }

                foreach (var signal in candlestick.PriceFunnels)
                {
                    if (signal.NumberOfFunnelCandlesticks > 3 &&
                        (candlestick.FractalTrend is Trend.Down || candlestick.PriceTrend is Trend.Down))
                    {
                        var candlestickFlagPoleId = pair?.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);

                        if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue)
                        {
                            var signalFound = indicator.Signals.FirstOrDefault(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));

                            if (signalFound is not null)
                            {
                                continue;
                            }

                            var signalIndicator = new Signal
                            {
                                OpenedAt = candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                Buy = 1,
                                EntryPrice = candlestick.ClosePrice
                            };

                            indicator.Signals.Add(signalIndicator);
                        }
                    }
                }
            }
            return indicator;
        }

        private static Indicator PrintFlagNestedRangeSignals(PairExtended pair)
        {
            var indicator = new Indicator { Name = "FlagNestedRangeSignals", PairName = pair.Symbol };

            if (pair.Candlesticks.Count == 0)
            {
                return indicator;
            }

            foreach (var candlestick in pair.Candlesticks)
            {
                foreach (var signal in candlestick.FlagsNestedCandlesticksRange)
                {
                    if (signal.NumberOfNestedCandlestickRanges > 3 &&
                        (candlestick.FractalTrend is Trend.Down || candlestick.PriceTrend is Trend.Down))
                    {
                        var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);
                        if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue)
                        {
                            var signalFound = indicator.Signals.FirstOrDefault(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));

                            if (signalFound is not null)
                            {
                                continue;
                            }

                            var signalIndicator = new Signal
                            {
                                OpenedAt = candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                Buy = 1,
                                EntryPrice = candlestick.ClosePrice
                            };

                            indicator.Signals.Add(signalIndicator);
                        }
                    }
                }

                foreach (var signal in candlestick.PriceFunnels)
                {
                    if (signal.NumberOfFunnelCandlesticks > 3 &&
                        (candlestick.FractalTrend is Trend.Down || candlestick.PriceTrend is Trend.Down))
                    {
                        var candlestickFlagPoleId = pair?.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);

                        if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue)
                        {
                            var signalFound = indicator.Signals.FirstOrDefault(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));

                            if (signalFound is not null)
                            {
                                continue;
                            }

                            var signalIndicator = new Signal
                            {
                                OpenedAt = candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                Buy = 1,
                                EntryPrice = candlestick.ClosePrice
                            };

                            indicator.Signals.Add(signalIndicator);
                        }
                    }
                }
            }
            return indicator;
        }

        private static Indicator PrintFunnelSignals(PairExtended pair)
        {
            var indicator = new Indicator { Name = "FunnelSignals", PairName = pair.Symbol };

            if (pair.Candlesticks.Count == 0)
            {
                return indicator;
            }

            foreach (var candlestick in pair.Candlesticks)
            {
                foreach (var signal in candlestick.PriceFunnels)
                {
                    if (signal.NumberOfFunnelCandlesticks > 3 &&
                        (candlestick.FractalTrend is Trend.Down || candlestick.PriceTrend is Trend.Down))
                    {
                        var candlestickFlagPoleId = pair?.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);

                        if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue)
                        {
                            var signalFound = indicator.Signals.FirstOrDefault(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));

                            if (signalFound is not null)
                            {
                                continue;
                            }

                            var signalIndicator = new Signal
                            {
                                OpenedAt = candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                Buy = 1,
                                EntryPrice = candlestick.ClosePrice
                            };

                            indicator.Signals.Add(signalIndicator);
                        }
                    }
                }
            }
            return indicator;
        }

        private static Indicator PrintResistanceBreakoutSignals(PairExtended pair)
        {
            var flagNestedCandlesticksBody = new Indicator { Name = "ResistanceBreakout", PairName = pair.Symbol };
            decimal? profit = 0;
            decimal? loss = 0;

            if (pair.Candlesticks.Count == 0)
            {
                return flagNestedCandlesticksBody;
            }

            foreach (var candlestick in pair.Candlesticks)
            {
                foreach (var signal in candlestick.ResistanceBreakouts.Where(e => e.IsBuy))
                {
                    var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);

                    var signalIndicator = new Signal
                    {
                        OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Buy = signal.IsBuy ? 1 : 0,
                        Sell = signal.ClosePosition ? 1 : 0,
                    };
                    flagNestedCandlesticksBody.Signals.Add(signalIndicator);

                    if (signal.ProfitInMoney > 0)
                    {
                        profit += signal.ProfitInMoney;
                    }

                    if (signal.LossInMoney < 0)
                    {
                        loss += signal.LossInMoney;
                    }
                }
            }
            decimal? profitAndLoss = profit + loss;

            return flagNestedCandlesticksBody;
        }

        private static void CalculateMarketStatistics(MarketStatistic marketStatistic, IEnumerable<PairExtended> selectedPairs, IEnumerable<CryptoFearAndGreedData> cryptoFearAndGreedData)
        {
            var cryptoFearAndGreedIndexDict = cryptoFearAndGreedData.ToDictionary(c => c.TimestampAsDateTime.Date, c => c);
            var dailyStatisticsDict = marketStatistic.DailyStatistics.GroupBy(ds => ds.Key.Date).ToDictionary(g => g.Key, g => g.Select(ds => ds.Value).ToList());

            Parallel.ForEach(selectedPairs, ParallelConfig.GetOptions(), pair =>
            {
                Parallel.ForEach(pair.Candlesticks.Where(c => c.EnhancedScans.Count > 0), candlestick =>
                {
                    KeyValuePair<DateTime, DailyStatistic> dailyStatisticFound = marketStatistic.DailyStatistics.FirstOrDefault(d => d.Key.Date == candlestick.CloseDate.Date);

                    if (!dailyStatisticsDict.TryGetValue(candlestick.CloseDate.Date, out var dailyStatistic))
                    {
                        candlestick.EnhancedScans.Clear();
                        return;
                    }

                    // Check if the pair is in the statistic
                    if (dailyStatisticFound.Value.PairsWithEnhancedScan.Contains(pair.Symbol))
                    {
                        if (!cryptoFearAndGreedIndexDict.TryGetValue(candlestick.CloseDate.Date, out var cryptoFearAndGreedIndex)
                                   || cryptoFearAndGreedIndex.ValueClassification == "ExtremeGreed"
                                   || cryptoFearAndGreedIndex.ValueClassification == "Greed")
                        {
                            candlestick.EnhancedScans.Clear();
                        }
                    }
                    else
                    {
                        // Clear enhanced scans if the pair is not in the statistic
                        candlestick.EnhancedScans.Clear();
                    }

                });
            });


            /*            Parallel.ForEach(selectedPairs, ParallelConfig.GetOptions(), pair =>
                        {
                            foreach (var candlestick in pair.Candlesticks.Where(c => c.EnhancedScans.Count > 0))
                            {
                                KeyValuePair<DateTime, DailyStatistic> dailyStatisticFound = marketStatistic.DailyStatistics.FirstOrDefault(d => d.Key.Date == candlestick.CloseDate.Date);

                                if (dailyStatisticFound.Equals(default(KeyValuePair<DateTime, DailyStatistic>)))
                                {
                                    candlestick.EnhancedScans.Clear();
                                    continue;
                                }

                                // Check if the pair is in the statistic
                                if (dailyStatisticFound.Value.PairsWithEnhancedScan.Contains(pair.Symbol))
                                {
                                    // Find crypto fear and greed index for the date
                                    var cryptoFearAndGreedIndex = cryptoFearAndGreedData.FirstOrDefault(c => c.TimestampAsDateTime.Date == candlestick.CloseDate.Date);

                                    // Clear enhanced scans based on value classification
                                    if (cryptoFearAndGreedIndex?.ValueClassification is null
                                        || cryptoFearAndGreedIndex.ValueClassification == "ExtremeGreed"
                                        || cryptoFearAndGreedIndex.ValueClassification == "Greed")
                                    {
                                        candlestick.EnhancedScans.Clear();
                                    }
                                }
                                else
                                {
                                    // Clear enhanced scans if the pair is not in the statistic
                                    candlestick.EnhancedScans.Clear();
                                }

                            }
                        });
            */
        }

        private async Task CalculateTechnicalIndicators(IEnumerable<PairExtended> pairs)
        {
            var cryptoFearAndGreedDataTask = mediator.Send(new GetCryptoFearAndGreedIndexQuery());

            BasicIndicatorExtension.Logger = logger;
            AdvancedIndicatorExtension.Logger = logger;
            PairStatisticsExtension.Logger = logger;

            Parallel.ForEach(pairs, ParallelConfig.GetOptions(), pair => pair.CalculateBasicIndicators());
            Parallel.ForEach(pairs, ParallelConfig.GetOptions(), pair => pair.CalculateSignalIndicators());

            var marketStatistic = await CountPairsWithEnhancedScanIsBuy(pairs);
            var cryptoFearAndGreedData = (await cryptoFearAndGreedDataTask).OrderByDescending(c => c.TimestampAsDateTime);
            CalculateMarketStatistics(marketStatistic, pairs, cryptoFearAndGreedData);

            // pairs.CalculatePairStatistics();
        }

        private async Task<List<PairExtended>> FormatAssetsPairsCandlesticks()
        {
            var fetchedAssetsTask = mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = mediator.Send(new GetPairsQuery());
            var fetchedCandlesticksTask = mediator.Send(new GetCandlesticksQuery());
            var fetchedPoolsTask = mediator.Send(new GetPoolsQuery());
            var fetchedDexCandlesticksTask = mediator.Send(new GetDexCandlesticksQuery());

            await Task.WhenAll(fetchedAssetsTask, fetchedPairsTask, fetchedCandlesticksTask, fetchedPoolsTask, fetchedDexCandlesticksTask);

            var assets = await fetchedAssetsTask;
            var pairs = (await fetchedPairsTask).Where(s => s.IsActive).ToList();
            var candlesticks = (await fetchedCandlesticksTask).ToList();
            var pools = (await fetchedPoolsTask).PoolToDomain().Where(p => p.IsActive);
            var dexCandlesticks = (await fetchedDexCandlesticksTask).DexCandlestickToDomain();

            pairs.AddRange(pools);
            candlesticks.AddRange(dexCandlesticks);

            pairs.MapPairsToAssets(assets);
            pairs.MapPairsToCandlesticks(candlesticks);

            return pairs;
        }

        private async Task<MarketStatistic> CountPairsWithEnhancedScanIsBuy(IEnumerable<PairExtended> pairs)
        {
            var marketStatistic = new MarketStatistic();

            foreach (var pair in pairs)
            {
                // Dictionary to keep track of dates where the pair has a candlestick record
                var datesWithCandlestick = new HashSet<DateTime>();

                foreach (var candlestick in pair.Candlesticks)
                {
                    // Add the date of the candlestick to the set
                    datesWithCandlestick.Add(candlestick.CloseDate);

                    // Check for enhanced scan and if it's a buy
                    if (candlestick.EnhancedScans.Count > 0 && candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsBuy == true)
                    {
                        if (!marketStatistic.DailyStatistics.TryGetValue(candlestick.CloseDate, out var dailyStat))
                        {
                            dailyStat ??= new DailyStatistic();
                            marketStatistic.DailyStatistics[candlestick.CloseDate] = dailyStat;
                        }

                        dailyStat.PairsWithEnhancedScan.Add(pair.Symbol);
                    }
                }

                // Update the NumberOfPairs for each date where this pair has a candlestick record
                foreach (var date in datesWithCandlestick)
                {
                    if (!marketStatistic.DailyStatistics.TryGetValue(date, out var dailyStat))
                    {
                        dailyStat ??= new DailyStatistic();
                        marketStatistic.DailyStatistics[date] = dailyStat;
                    }

                    dailyStat.NumberOfPairs++;
                }
            }

            marketStatistic.CalculateAndFilterPercentages(50);

            var baseDirectory = GetBaseDirectory();
            string jsonFileName = Path.Combine(baseDirectory, $"{nameof(MarketStatistic)}.json");
            await JsonHelper.SerializeToJson(marketStatistic, jsonFileName);

            return marketStatistic;
        }
    }
}
