using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Helpers;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.Indicators.Advanced;
using TechnicalAnalysis.CommonModels.JsonOutput;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Utilities;
using Indicator = TechnicalAnalysis.CommonModels.JsonOutput.Indicator;

namespace TechnicalAnalysis.Application.Services
{
    public class AnalysisService(ILogger<AnalysisService> logger, IMediator mediator, IConfiguration configuration, IRedisRepository redisRepository)
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

            CalculateTechnicalIndicators(pairs);

            var filteredPairs = pairs.OrderByDescending(pair => pair.CreatedAt)
                               .Select(pair =>
                               {
                                   pair.Candlesticks = pair.Candlesticks
                                                   .Where(c => c.EnhancedScans.Count > 0)
                                                   .OrderByDescending(c => c.CloseDate)
                                                   .GroupBy(c => c.PoolOrPairId)
                                                   .Select(group => group.First()) // Take the first item of each group
                                                   .ToList();
                                   return pair;
                               })
                               .Where(pair => pair.Candlesticks.Count > 0);

            return filteredPairs;
        }

        public async Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(string pairName, Timeframe timeframe)
        {
            var fetchedPairs = await FormatAssetsPairsCandlesticks();
            var cachedPairs = new List<PairExtended>();

            foreach (var pair in fetchedPairs)
            {
                var cachedPair = await redisRepository.GetRecordAsync<PairExtended>(pair.Symbol);
                if (cachedPair?.HasCalculateDailyTechnicalAnalysis == true)
                {
                    cachedPairs.Add(cachedPair);
                }
            }

            var pairsToCalculate = fetchedPairs.Except(cachedPairs).ToList();
            if (pairsToCalculate.Count > 0)
            {
                CalculateTechnicalIndicators(pairsToCalculate);
                foreach (var pair in pairsToCalculate)
                {
                    await redisRepository.SetRecordAsync(pair.Symbol, pair, null, null);
                }
            }

            var allPairs = cachedPairs.Concat(pairsToCalculate);

            var selectedPairs = allPairs.Where(p => p.Symbol.Equals(pairName, StringComparison.InvariantCultureIgnoreCase));
            await CalculateMarketStatistics(allPairs, selectedPairs);

            var positionsCloseOneByOne = selectedPairs.AverageDownStrategyCloseOneByOnBasedInFractalBreak();
            var positionsCloseAll = selectedPairs.AverageDownStrategyCloseAllBasedInFractalBreak();

            Indicator enhancedScanPositionsCloseOneByOne = CalculateEnhancedScanSignal(positionsCloseOneByOne, "EnhanchedScan_CloseOneByOne");
            Indicator enhancedScanPositionsCloseAll = CalculateEnhancedScanSignal(positionsCloseAll, "EnhanchedScan_CloseAll");
            var indicatorReports = new List<Indicator>()
            {
                enhancedScanPositionsCloseOneByOne,
                enhancedScanPositionsCloseAll
            };

            foreach (var pair in selectedPairs)
            {
                pair.Candlesticks = pair.Candlesticks.OrderBy(c => c.OpenDate).ToList();

                Indicator enhancedScan = CalculateEnhancedScanAllSignals(pair);
                Indicator fractalTrend = PrintFractalTrend(pair);
                Indicator lowestHighLowestLowFractalSignals = PrintLowestHighSignals(pair);
                Indicator flagNestedBodySignals = PrintFlagNestedBodySignals(pair);
                Indicator pivotSignals = PrintPivotSignals(pair);
                Indicator resistanceSignals = PrintResistanceBreakoutSignals(pair);
                Indicator stPatterns = CalculateStPatternSignals(pair);
                Indicator closeBelowPivotSignal = CalculateCandlestickCloseBelowPivotPrice(pair);

                indicatorReports.Add(enhancedScan);
                indicatorReports.Add(fractalTrend);
                indicatorReports.Add(lowestHighLowestLowFractalSignals);
                indicatorReports.Add(flagNestedBodySignals);
                indicatorReports.Add(pivotSignals);
                indicatorReports.Add(resistanceSignals);
                indicatorReports.Add(stPatterns);
                indicatorReports.Add(closeBelowPivotSignal);
            }

            foreach (var indicator in indicatorReports)
            {
                var outputPair = selectedPairs.FirstOrDefault()?.ToOutputContract();

                var baseDirectory = GetBaseDirectory();
                string candlestickFileName = Path.Combine(baseDirectory, $"{outputPair?.Symbol}-candlesticks.json");
                await JsonHelper.SerializeToJson(outputPair, candlestickFileName);
                string signalFileName = Path.Combine(baseDirectory, $"{outputPair?.Symbol}-{indicator.Name}.json");
                await JsonHelper.SerializeToJsonArray(indicator, signalFileName);
            }

            return selectedPairs;
        }

        private string GetBaseDirectory()
        {
            var baseDirectory = configuration.GetSection("OutputFolder:Path").Value;

            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                return string.Empty;
            }

            return baseDirectory;
        }

        private static Indicator CalculateEnhancedScanSignal(IEnumerable<Position> positionsStrategy, string name)
        {
            var enhancedScan = new Indicator { Name = name };
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

            foreach (var candlestick in pair.Candlesticks.Where(c => c.CloseRelativeToPivots.FirstOrDefault()?.NumberOfConsecutiveCandlestickBelowPivot >= 5))
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
            var enhancedScan = new Indicator { Name = "EnhancedScanAllSignals" };

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

        private static Indicator CalculateStPatternSignals(PairExtended pair)
        {
            var stPatternSignals = new Indicator { Name = "StPatternSignals" };

            foreach (var candlestick in pair.Candlesticks.Where(c => c.StPatternSignals.FirstOrDefault()?.NumberOfSignal == 1))
            {
                var signalIndicator = new Signal
                {
                    OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Buy = 1,
                };
                stPatternSignals.Signals.Add(signalIndicator);
            }

            return stPatternSignals;
        }

        private static Indicator PrintFractalTrend(PairExtended pair)
        {
            var fractalTrend = new Indicator { Name = "FractalTrend" };
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
            var lowestHighLowestLowFractal = new Indicator { Name = "lowestHighLowestLowFractal" };

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
            var indicator = new Indicator { Name = "FlagNestedBodySignals" };

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

        private static Indicator PrintResistanceBreakoutSignals(PairExtended pair)
        {
            var flagNestedCandlesticksBody = new Indicator { Name = "ResistanceBreakout" };
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

        private static Indicator PrintPivotSignals(PairExtended pair)
        {
            var pivot = new Indicator { Name = "PivotPointSignals" };
            var candlesticks = pair?.Candlesticks;

            // Start the loop from the second candlestick to avoid index -1
            for (int i = 1; i < candlesticks?.Count; i++)
            {
                var candlestick = candlesticks[i];
                foreach (var standardPivotPoint in candlestick.CloseRelativeToPivots.Where(c => c.NumberOfConsecutiveCandlestickBelowPivot > 3))
                {
                    var signalIndicator = new Signal
                    {
                        OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Buy = 1
                    };
                    pivot.Signals.Add(signalIndicator);
                }
            }
            return pivot;
        }

        private async Task CalculateMarketStatistics(IEnumerable<PairExtended> pairs, IEnumerable<PairExtended> selectedPairs)
        {
            var marketStatistic = await CountPairsWithEnhancedScanIsBuy(pairs);

            if (selectedPairs is null)
            {
                return;
            }

            foreach (var pair in selectedPairs)
            {
                // Clear EnhancedScans for each candlestick in the current pair
                foreach (var candlestick in pair.Candlesticks)
                {
                    candlestick.EnhancedScans.Clear();
                }

                // Create a dictionary for candlesticks by date for the current pair
                var candlesticksByDate = pair.Candlesticks.ToDictionary(c => c.CloseDate, c => c);

                // Add new EnhancedScan entries based on marketStatistic
                foreach (var kvp in marketStatistic.DailyStatistics)
                {
                    if (kvp.Value.PairsWithEnhancedScan.Contains(pair.Symbol)
                        && candlesticksByDate.TryGetValue(kvp.Key, out var candlestick))
                    {
                        candlestick.EnhancedScans.Add(new EnhancedScan(candlestick.PrimaryId) { EnhancedScanIsBuy = true });
                    }
                }
            }
        }

        private void CalculateTechnicalIndicators(IEnumerable<PairExtended> pairs)
        {
            BasicIndicatorExtension.Logger = logger;
            AdvancedIndicatorExtension.Logger = logger;
            PairStatisticsExtension.Logger = logger;

            Parallel.ForEach(pairs, ParallelOption.GetOptions(), pair => pair.CalculateBasicIndicators());
            Parallel.ForEach(pairs, ParallelOption.GetOptions(), pair => pair.CalculateSignalIndicators());
            Parallel.ForEach(pairs, ParallelOption.GetOptions(), pair => pair.HasCalculateDailyTechnicalAnalysis = true);

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
