using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.OutputContract;
using TechnicalAnalysis.Domain.Contracts.Output;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Utilities;
using Indicator = TechnicalAnalysis.CommonModels.OutputContract.Indicator;

namespace TechnicalAnalysis.Application.Services
{
    public class AnalysisService(ILogger<AnalysisService> logger, IMediator mediator, IConfiguration configuration)
        : IAnalysisService
    {
        public async Task<List<EnhancedPairResult>> GetEnhancedPairResultsAsync(DataProvider provider, HttpContext? httpContext = null)
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
                return [];
            }

            await CalculateTechnicalIndicators(pairs);

            var pairsWithEnhancedScanCandlesticks = FilterPairs(pairs, c => c.EnhancedScans.Exists(e => e.EnhancedScanIsLong || e.EnhancedScanIsShort));

            var filteredEnhancedScanPairs = pairsWithEnhancedScanCandlesticks
                .Select(pair => new EnhancedPairResult
                {
                    Symbol = pair.Symbol,
                    EnhancedScans = pair.Candlesticks
                        //  .Where(f => f.EnhancedScans.Exists(e => e.EnhancedScanIsLong || e.EnhancedScanIsShort))
                        .SelectMany(c => c.EnhancedScans, (candlestick, enhancedScan) => new EnhancedScanGroup
                        {
                            CandlestickCloseDate = candlestick.CloseDate,
                            OrderOfLongSignal = enhancedScan?.OrderOfLongSignal,
                            OrderOfShortSignal = enhancedScan?.OrderOfShortSignal,
                            CandlestickClosePrice = candlestick.ClosePrice,
                            DaysFromAllTimeHigh = candlestick.DaysFromAllTimeHigh,
                            PercentageFromAllTimeHigh = candlestick.PercentageFromAllTimeHigh
                        })
                        .OrderByDescending(es => es.CandlestickCloseDate)
                        .ToList()
                })
                .OrderByDescending(result => result.EnhancedScans.FirstOrDefault()?.CandlestickCloseDate)
                .ToList();

            return filteredEnhancedScanPairs;
        }

        private static List<PairExtended> FilterPairs(List<PairExtended> pairs, Func<CandlestickExtended, bool> predicate)
        {
            return pairs.OrderByDescending(pair => pair.CreatedAt)
                        .Select(pair =>
                        {
                            pair.Candlesticks = pair.Candlesticks
                                                .Where(predicate)
                                                .OrderByDescending(c => c.CloseDate)
                                                .GroupBy(c => c.PoolOrPairId)
                                                .Select(group => group.First())
                                                .ToList();
                            return pair;
                        })
                        .Where(pair => pair.Candlesticks.Count > 0).ToList();
        }

        public async Task<List<PairExtended>> GetIndicatorsByPairNamesAsync(List<string> pairNames, Timeframe timeframe, HttpContext? httpContext = null)
        {
            var fetchedPairs = await FormatAssetsPairsCandlesticks();
            var selectedPairs = fetchedPairs.Where(p => pairNames.Contains(p.Symbol, StringComparer.InvariantCultureIgnoreCase)).ToList();

            if (selectedPairs.Count == 0)
            {
                return [];
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
                    CalculateEnhancedScanLongSignal(positionsCloseOneByOne, "EnhancedScan_CloseOneByOne", selectedPair.Symbol),
                    CalculateEnhancedScanLongSignal(positionsCloseAll, "EnhancedScan_CloseAll", selectedPair.Symbol),
                    CalculateEnhancedScanAllSignals(selectedPair),
                    PrintFractalTrend(selectedPair),
                    PrintLowestHighFractalSignals(selectedPair),
                    PrintFlagNestedBodySignals(selectedPair),
                    PrintFlagNestedRangeSignals(selectedPair),
                    PrintFunnelSignals(selectedPair),
                    PrintAllKindOfRanges(selectedPair),
                    PrintResistanceBreakoutSignals(selectedPair),
                    CalculateCandlestickCloseBelowPivotPrice(selectedPair),
                    CalculateEnhancedScanShortSignal(selectedPair)
                };

                indicatorReportsPerPair.Add(selectedPair, indicatorReports);
            }

            var baseDirectory = GetBaseDirectory();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                return [];
            }

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

            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                return string.Empty;
            }

            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            return baseDirectory;
        }

        private static Indicator CalculateEnhancedScanLongSignal(List<Position> positionsStrategy, string indicatorName, string pairSymbol)
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

        private static Indicator CalculateEnhancedScanShortSignal(PairExtended pair)
        {
            var indicator = new Indicator { Name = "EnhancedShortSignal" };

            foreach (var candlestick in pair.Candlesticks.Where(c => c.EnhancedScans.FirstOrDefault()?.OrderOfShortSignal == 1))
            {
                var signalIndicator = new Signal
                {
                    OpenedAt = candlestick.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Sell = 1,
                };
                indicator.Signals.Add(signalIndicator);
            }

            return indicator;
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
                .Where(c => c.EnhancedScans?.FirstOrDefault()?.OrderOfLongSignal >= 0))
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

            for (int i = 0; i < pair.Candlesticks.Count; i++)
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

        private static Indicator PrintLowestHighFractalSignals(PairExtended pair)
        {
            var lowestHighLowestLowFractal = new Indicator { Name = "lowestHighLowestLowFractal", PairName = pair.Symbol };

            var lastSignal = string.Empty;
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = i - 1 >= 0 ? pair.Candlesticks[i - 1] : null;

                if (candlestick1 == null)
                {
                    continue;
                }

                if (candlestick1.FractalTrend == Trend.Up
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

                if (candlestick1.FractalTrend == Trend.Down
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
                foreach (var signal in candlestick.FlagsNestedCandlesticksBody.Where(s => s.NumberOfNestedCandlestickBodies >= 3))
                {
                    var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);
                    if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue / 2)
                    {
                        var signalFound = indicator.Signals.Find(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));
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
                foreach (var signal in candlestick.FlagsNestedCandlesticksRange.Where(s => s.NumberOfNestedCandlestickRanges >= 3))
                {
                    var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);
                    if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue / 2)
                    {
                        var signalFound = indicator.Signals.Find(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));
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
                    if (signal.NumberOfFunnelCandlesticks >= 3)
                    {
                        var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);

                        if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue / 2)
                        {
                            var signalFound = indicator.Signals.Find(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));

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

        private static Indicator PrintAllKindOfRanges(PairExtended pair)
        {
            var indicator = new Indicator { Name = "PrintAllKindOfRanges", PairName = pair.Symbol };

            if (pair.Candlesticks.Count == 0)
            {
                return indicator;
            }

            foreach (var candlestick in pair.Candlesticks)
            {
                foreach (var signal in candlestick.PriceFunnels)
                {
                    if (signal.NumberOfFunnelCandlesticks >= 3)
                    {
                        var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);

                        if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue / 2)
                        {
                            var signalFound = indicator.Signals.Find(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));

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

            foreach (var candlestick in pair.Candlesticks)
            {
                foreach (var signal in candlestick.FlagsNestedCandlesticksBody.Where(s => s.NumberOfNestedCandlestickBodies >= 3))
                {
                    var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);
                    if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue / 2)
                    {
                        var signalFound = indicator.Signals.Find(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));
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

            foreach (var candlestick in pair.Candlesticks)
            {
                foreach (var signal in candlestick.FlagsNestedCandlesticksRange.Where(s => s.NumberOfNestedCandlestickRanges >= 3))
                {
                    var candlestickFlagPoleId = pair.Candlesticks.Find(c => c.PrimaryId == signal.FlagPoleCandlestickId);
                    if (candlestickFlagPoleId?.Range > candlestickFlagPoleId?.AverageTrueRanges?.FirstOrDefault()?.AverageTrueRangeValue / 2)
                    {
                        var signalFound = indicator.Signals.Find(s => s.OpenedAt == candlestickFlagPoleId.OpenDate.ToString("yyyy-MM-dd HH:mm:ss"));
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

        private static void CalculateEnhancedIsLongBasedOnStatistics(MarketStatistic marketStatistic,
            List<PairExtended> selectedPairs,
            Func<PairExtended, bool> filterPredicate)
        {
            var dailyStatisticsDict = marketStatistic.DailyStatistics
                .GroupBy(ds => ds.Key.Date)
                .ToDictionary(g => g.Key, g => g.Select(ds => ds.Value).ToList());

            Parallel.ForEach(selectedPairs.Where(filterPredicate),
                ParallelConfig.GetOptions(), pair =>
            {
                foreach (var candlestick in pair.Candlesticks.Where(c => c.EnhancedScans.Exists(e => e.EnhancedScanIsLong)))
                {
                    if (candlestick.CloseDate.Date == new DateTime(2023, 01, 03).Date
                        && string.Equals(pair.Symbol, "AAPL", StringComparison.InvariantCultureIgnoreCase))
                    {
                    }

                    if (!dailyStatisticsDict.TryGetValue(candlestick.CloseDate.Date, out var dailyStatistics))
                    {
                        var enchancedScanLong = candlestick.EnhancedScans.FirstOrDefault();
                        if (enchancedScanLong is not null)
                        {
                            enchancedScanLong.EnhancedScanIsLong = false;
                        }

                        continue;
                    }

                    var isPairInStatistic = dailyStatistics.Exists(ds => ds.PairsWithEnhancedScan.Any(s => string.Equals(s, pair.Symbol, StringComparison.InvariantCultureIgnoreCase)));
                    if (!isPairInStatistic)
                    {
                        var enchancedScanLong = candlestick.EnhancedScans.FirstOrDefault();
                        if (enchancedScanLong is not null)
                        {
                            enchancedScanLong.EnhancedScanIsLong = false;
                        }
                    }
                }
            });

        }

        private async Task CalculateTechnicalIndicators(List<PairExtended> pairs)
        {
            var cryptoFearAndGreedDataTask = mediator.Send(new GetCryptoFearAndGreedIndexQuery());
            var stockFearAndGreedDataTask = mediator.Send(new GetStockFearAndGreedIndexQuery());

            BasicIndicatorExtension.Logger = logger;
            AdvancedIndicatorExtension.Logger = logger;
            PairStatisticsExtension.Logger = logger;

            var cryptoFearAndGreedData = (await cryptoFearAndGreedDataTask).OrderByDescending(c => c.DateTime).ToList();
            var cryptoFearAndGreedDataPerDatetime = cryptoFearAndGreedData.ToDictionary(c => c.DateTime.Date, c => c);

            var stockFearAndGreedData = (await stockFearAndGreedDataTask).OrderByDescending(c => c.DateTime).ToList();
            var stockFearAndGreedDataPerDatetime = stockFearAndGreedData.ToDictionary(c => c.DateTime.Date, c => c);

            Parallel.ForEach(pairs, ParallelConfig.GetOptions(), pair => pair.CalculateBasicIndicators());
            Parallel.ForEach(pairs, ParallelConfig.GetOptions(), pair => pair.CalculateSignalIndicators(cryptoFearAndGreedDataPerDatetime, stockFearAndGreedDataPerDatetime));

            var cryptoMarketStatistic = await GetCryptoPairsWithEnhancedScanIsLong(pairs);
            var etfStockMarketStatistic = await GetEtfStockPairWithEnhancedScanIsLong(pairs);

            CalculateEnhancedIsLongBasedOnStatistics(cryptoMarketStatistic, pairs, p => p.Provider == DataProvider.Binance
            || p.Provider == DataProvider.Uniswap
            || p.Provider == DataProvider.Pancakeswap);

            CalculateEnhancedIsLongBasedOnStatistics(etfStockMarketStatistic, pairs, p => p.Provider == DataProvider.Alpaca);

            // pairs.CalculatePairStatistics();
        }

        private async Task<List<PairExtended>> FormatAssetsPairsCandlesticks()
        {
            var fetchedAssetsTask = mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = mediator.Send(new GetPairsQuery());
            var fetchedCandlesticksTask = mediator.Send(new GetCandlesticksQuery());
            var fetchedPoolsTask = mediator.Send(new GetPoolsQuery());
            var fetchedDexCandlesticksTask = mediator.Send(new GetDexCandlesticksQuery());

            var pairs = (await fetchedPairsTask).Where(s => s.IsActive).ToList();
            var pools = (await fetchedPoolsTask).PoolToDomain().Where(p => p.IsActive);

            pairs.AddRange(pools);

            var candlesticks = await fetchedCandlesticksTask;
            var dexCandlesticks = (await fetchedDexCandlesticksTask).DexCandlestickToDomain();

            candlesticks.AddRange(dexCandlesticks);

            var assets = await fetchedAssetsTask;

            pairs.MapPairsToAssets(assets);
            pairs.MapPairsToCandlesticks(candlesticks);

            return pairs;
        }

        private async Task<MarketStatistic> GetCryptoPairsWithEnhancedScanIsLong(List<PairExtended> pairs)
        {
            var cryptoMarketStatistic = new MarketStatistic();

            foreach (var pair in pairs.Where(p => p.Provider is DataProvider.Binance
                        or DataProvider.Uniswap
                        or DataProvider.Pancakeswap))
            {
                // Dictionary to keep track of dates where the pair has a candlestick record
                var datesWithCandlestick = new HashSet<DateTime>();

                foreach (var candlestick in pair.Candlesticks)
                {
                    // Add the date of the candlestick to the set
                    datesWithCandlestick.Add(candlestick.CloseDate);

                    // Check for enhanced scan and if it's a buy
                    if (candlestick.EnhancedScans.Count > 0 && candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong == true)
                    {
                        if (!cryptoMarketStatistic.DailyStatistics.TryGetValue(candlestick.CloseDate, out var dailyStatistic))
                        {
                            dailyStatistic ??= new DailyStatistic();
                            cryptoMarketStatistic.DailyStatistics[candlestick.CloseDate] = dailyStatistic;
                        }

                        dailyStatistic.PairsWithEnhancedScan.Add(pair.Symbol);
                    }
                }

                // Update the NumberOfPairs for each date where this pair has a candlestick record
                foreach (var date in datesWithCandlestick)
                {
                    if (!cryptoMarketStatistic.DailyStatistics.TryGetValue(date, out var dailyStat))
                    {
                        dailyStat ??= new DailyStatistic();
                        cryptoMarketStatistic.DailyStatistics[date] = dailyStat;
                    }

                    dailyStat.NumberOfPairs++;
                }
            }

            cryptoMarketStatistic.CalculateAndFilterPercentages(Constants.CryptoThresholdPercentage);

            var baseDirectory = GetBaseDirectory();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                return cryptoMarketStatistic;
            }

            string jsonFileName = Path.Combine(baseDirectory, $"{nameof(GetCryptoPairsWithEnhancedScanIsLong)}-{nameof(MarketStatistic)}.json");
            await JsonHelper.SerializeToJson(cryptoMarketStatistic, jsonFileName);

            return cryptoMarketStatistic;
        }

        private async Task<MarketStatistic> GetEtfStockPairWithEnhancedScanIsLong(List<PairExtended> pairs)
        {
            var etfStockMarketStatistic = new MarketStatistic();

            foreach (var pair in pairs.Where(p => p.Provider is DataProvider.Alpaca))
            {
                // Dictionary to keep track of dates where the pair has a candlestick record
                var datesWithCandlestick = new HashSet<DateTime>();

                foreach (var candlestick in pair.Candlesticks)
                {
                    // Add the date of the candlestick to the set
                    datesWithCandlestick.Add(candlestick.CloseDate);

                    // Check for enhanced scan and if it's a buy
                    if (candlestick.EnhancedScans.Count > 0 && candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong == true)
                    {
                        if (!etfStockMarketStatistic.DailyStatistics.TryGetValue(candlestick.CloseDate, out var dailyStatistic))
                        {
                            dailyStatistic ??= new DailyStatistic();
                            etfStockMarketStatistic.DailyStatistics[candlestick.CloseDate] = dailyStatistic;
                        }

                        dailyStatistic.PairsWithEnhancedScan.Add(pair.Symbol);
                    }
                }

                // Update the NumberOfPairs for each date where this pair has a candlestick record
                foreach (var date in datesWithCandlestick)
                {
                    if (!etfStockMarketStatistic.DailyStatistics.TryGetValue(date, out var dailyStat))
                    {
                        dailyStat ??= new DailyStatistic();
                        etfStockMarketStatistic.DailyStatistics[date] = dailyStat;
                    }

                    dailyStat.NumberOfPairs++;
                }
            }

            etfStockMarketStatistic.CalculateAndFilterPercentages(Constants.EtfThresholdPercentage);

            var baseDirectory = GetBaseDirectory();
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                return etfStockMarketStatistic;
            }

            string jsonFileName = Path.Combine(baseDirectory, $"{nameof(GetEtfStockPairWithEnhancedScanIsLong)}-{nameof(MarketStatistic)}.json");
            await JsonHelper.SerializeToJson(etfStockMarketStatistic, jsonFileName);

            return etfStockMarketStatistic;
        }

        public async Task<List<AssetRanking>> GetLayerOneAssetsAsync()
        {
            var fetchedAssets = await mediator.Send(new GetAssetsRankingQuery());
            return fetchedAssets.Where(a => a.AssetType is ProductType.Layer1).OrderByDescending(a => a.CreatedDate).ToList();
        }
    }
}
