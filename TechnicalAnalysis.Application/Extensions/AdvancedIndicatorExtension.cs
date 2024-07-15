using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.Indicators.Advanced;
using TechnicalAnalysis.CommonModels.Indicators.Basic;
using TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class AdvancedIndicatorExtension
    {
        public static ILogger Logger { get; set; }

        public static void CalculateSignalIndicators(this PairExtended pair,
            Dictionary<DateTime, FearAndGreedModel> cryptoFearAndGreedDataPerDatetime,
            Dictionary<DateTime, FearAndGreedModel> stockFearAndGreedDataPerDatetime)
        {
            Logger.LogInformation("Pair details - {PairPropertyName}: {PairName}, " +
                "{BaseAssetContractPropertyName}: {BaseAssetContract}, " +
                "{BaseAssetNamePropertyName}: {BaseAssetName}, " +
                "{QuoteAssetContractPropertyName}: {QuoteAssetContract}, " +
                "{QuoteAssetNamePropertyName}: {QuoteAssetName}",
                nameof(pair.Symbol), pair.Symbol,
                nameof(pair.BaseAssetContract), pair.BaseAssetContract,
                nameof(pair.BaseAssetName), pair.BaseAssetName,
                nameof(pair.QuoteAssetContract), pair.QuoteAssetContract,
                nameof(pair.QuoteAssetName), pair.QuoteAssetName);

            pair.Candlesticks = pair.Candlesticks
                .Where(candlestick => candlestick.OpenPrice.HasValue
                    && candlestick.HighPrice.HasValue
                    && candlestick.ClosePrice.HasValue
                    && candlestick.LowPrice.HasValue)
                .OrderBy(candlestick => candlestick.CloseDate)
                .ToList();

            CalculateCandlestickPatterns(pair);
            CalculatePriceFunnel(pair);
            CalculateBollingerBandsFunnel(pair);
            CalculateFlagNestedCandlesticksBody(pair);
            CalculateFlagNestedCandlesticksRange(pair);
            CalculateFractalTrendBasedOnPreviousFractals(pair);
            CalculatePriceTrendBasedOnFractalPattern(pair);
            CalculateConsecutiveClosePriceBelowPivot(pair);
            CalculateWilliamsVixFix(pair);
            CalculateHighestWilliamsVixFixValue(pair);

            CalculateEnchancedLong(pair, cryptoFearAndGreedDataPerDatetime, stockFearAndGreedDataPerDatetime);
            CalculateEnchancedShort(pair, cryptoFearAndGreedDataPerDatetime, stockFearAndGreedDataPerDatetime);
            CalculateResistanceBreakout(pair);
            CalculateVerticalHorizontalFilterRange(pair);
        }

        private static void CalculateWilliamsVixFix(PairExtended pair)
        {
            foreach (var candlestick in pair.Candlesticks)
            {
                var highestClose = candlestick.Highests.Find(c => c.PriceType == PriceType.Close && c.Period == 22);

                if (highestClose is null || highestClose.Value is 0)
                {
                    continue;
                }

                var vixFixValue = (highestClose.Value - candlestick.LowPrice) / highestClose.Value * 100;
                candlestick.VixFixes.Add(new VixFix(candlestick.PrimaryId) { Period = 22, Value = vixFixValue });
            }
        }

        private static void CalculateHighestWilliamsVixFixValue(PairExtended pair)
        {
            const int period = 5;

            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                int startIndex = Math.Max(i - (period - 1), 0);
                var group = pair.Candlesticks.GetRange(startIndex, Math.Min(period, i - startIndex + 1));
                var vixFixHighestHigh = group.Max(candlestickGroup => candlestickGroup.VixFixes.FirstOrDefault()?.Value ?? 0);

                var candlestick = pair.Candlesticks[i];
                candlestick.Highests.Add(new Highest(candlestick.PrimaryId)
                {
                    Period = period,
                    Value = vixFixHighestHigh,
                    PriceType = PriceType.VixFix
                });
            }
        }

        private static void CalculateConsecutiveClosePriceBelowPivot(PairExtended pair)
        {
            var latestOrderedCandlestick = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();

            int count = 0;
            for (int i = 1; i < latestOrderedCandlestick.Count; i++)
            {
                CandlestickExtended previousCandlestick = latestOrderedCandlestick[i - 1];
                CandlestickExtended currentCandlestick = latestOrderedCandlestick[i];

                if (currentCandlestick.ClosePrice <= currentCandlestick.StandardPivotPoints.FirstOrDefault()?.PivotPoint &&
                    previousCandlestick.ClosePrice <= previousCandlestick.StandardPivotPoints.FirstOrDefault()?.PivotPoint)
                {
                    count++;
                    currentCandlestick.CloseRelativeToPivots.Add(new CloseRelativeToPivot(currentCandlestick.PrimaryId)
                    {
                        NumberOfConsecutiveCandlestickBelowPivot = count,
                    });
                }
                else
                {
                    count = 0;
                }
            }
        }

        private static void CalculatePriceFunnel(PairExtended pair)
        {
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                CandlestickExtended initialCandlestick = pair.Candlesticks[i];
                int count = 0;

                for (int j = i + 1; j < pair.Candlesticks.Count; j++)
                {
                    CandlestickExtended funnelCandlestick = pair.Candlesticks[j];

                    if (funnelCandlestick.HighPrice <= initialCandlestick.HighPrice &&
                        funnelCandlestick.LowPrice >= initialCandlestick.LowPrice)
                    {
                        count++;
                        funnelCandlestick.PriceFunnels.Add(new PriceFunnel(funnelCandlestick.PrimaryId)
                        {
                            IsFunnel = true,
                            NumberOfFunnelCandlesticks = count,
                            HighestPriceOfFunnel = initialCandlestick.HighPrice,
                            FlagPoleCandlestickId = initialCandlestick.PrimaryId
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private static void CalculateBollingerBandsFunnel(PairExtended pair)
        {
            int count = 0;

            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = i - 1 >= 0 ? pair.Candlesticks[i - 1] : null;

                if (candlestick1?.BollingerBands?.FirstOrDefault()?.BandWidth >= candlestick?.BollingerBands?.FirstOrDefault()?.BandWidth
                    && candlestick1.BollingerBands?.FirstOrDefault()?.UpperBand - candlestick1.BollingerBands?.FirstOrDefault()?.LowerBand
                    >= candlestick.BollingerBands?.FirstOrDefault()?.UpperBand - candlestick.BollingerBands?.FirstOrDefault()?.LowerBand
                    && candlestick.Adxs?.FirstOrDefault()?.PlusDi <= 25
                    && candlestick.Adxs?.FirstOrDefault()?.MinusDi <= 25)
                {
                    count++;

                    candlestick.BollingerBandsFunnels.Add(new BollingerBandsFunnel(candlestick.PrimaryId)
                    {
                        IsBollingerBandsFunnel = true,
                        NumberOfBollingerBandsFunnelCandlesticks = count,
                        HighestPriceOfFunnel = candlestick.HighPrice
                    });
                }
                else
                {
                    count = 0;
                }
            }
        }

        private static void CalculateFlagNestedCandlesticksRange(PairExtended pair)
        {
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                CandlestickExtended initialCandlestick = pair.Candlesticks[i];
                int count = 0;

                for (int j = i + 1; j < pair.Candlesticks.Count; j++)
                {
                    CandlestickExtended nestedCandlestick = pair.Candlesticks[j];

                    if (nestedCandlestick.HighPrice <= initialCandlestick.HighPrice &&
                        nestedCandlestick.LowPrice >= initialCandlestick.LowPrice)
                    {
                        count++;
                        nestedCandlestick.FlagsNestedCandlesticksRange.Add(new FlagNestedCandlestickRange(nestedCandlestick.PrimaryId)
                        {
                            IsFlag = true,
                            NumberOfNestedCandlestickRanges = count,
                            FlagPoleCandlestickId = initialCandlestick.PrimaryId
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private static void CalculateFlagNestedCandlesticksBody(PairExtended pair)
        {
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                CandlestickExtended initialCandlestick = pair.Candlesticks[i];
                int count = 0;

                //For testing purposes
                if (initialCandlestick.CloseDate.Date == new DateTime(2023, 12, 13).Date && pair.BaseAssetName == "ADA")
                {
                }

                for (int j = i + 1; j < pair.Candlesticks.Count; j++)
                {
                    CandlestickExtended nestedCandlestick = pair.Candlesticks[j];

                    if (nestedCandlestick.ClosePrice <= initialCandlestick.HighPrice &&
                        nestedCandlestick.OpenPrice <= initialCandlestick.HighPrice &&
                        nestedCandlestick.ClosePrice >= initialCandlestick.LowPrice &&
                        nestedCandlestick.OpenPrice >= initialCandlestick.LowPrice)
                    {
                        count++;
                        nestedCandlestick.FlagsNestedCandlesticksBody.Add(new FlagNestedCandlestickBody(nestedCandlestick.PrimaryId)
                        {
                            IsFlag = true,
                            NumberOfNestedCandlestickBodies = count,
                            FlagPoleCandlestickId = initialCandlestick.PrimaryId
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private static void CalculateFractalTrendBasedOnPreviousFractals(PairExtended pair)
        {
            CandlestickExtended? currentBullFractalCandlestick = null;
            CandlestickExtended? previousBullFractalCandlestick = null;
            CandlestickExtended? secondPreviousBullFractalCandlestick = null;

            CandlestickExtended? currentBearFractalCandlestick = null;
            CandlestickExtended? previousBearFractalCandlestick = null;
            CandlestickExtended? secondPreviousBearFractalCandlestick = null;

            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick3 = i - 2 >= 0 ? pair.Candlesticks[i - 2] : null;

                if (candlestick3 is null)
                {
                    continue;
                }

                if (candlestick3.Fractals.Exists(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2))
                {
                    if (currentBullFractalCandlestick is null || candlestick3.PrimaryId > currentBullFractalCandlestick?.PrimaryId)
                    {
                        secondPreviousBullFractalCandlestick = previousBullFractalCandlestick;
                        previousBullFractalCandlestick = currentBullFractalCandlestick;
                        currentBullFractalCandlestick = candlestick3;
                    }
                }

                if (candlestick3.Fractals.Exists(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2))
                {
                    if (currentBearFractalCandlestick is null || candlestick3.PrimaryId > currentBearFractalCandlestick?.PrimaryId)
                    {
                        secondPreviousBearFractalCandlestick = previousBearFractalCandlestick;
                        previousBearFractalCandlestick = currentBearFractalCandlestick;
                        currentBearFractalCandlestick = candlestick3;
                    }
                }

                if (currentBullFractalCandlestick is null
                    || previousBullFractalCandlestick is null
                    || currentBearFractalCandlestick is null
                    || previousBearFractalCandlestick is null)
                {
                    continue;
                }

                if (currentBullFractalCandlestick?.LowPrice > previousBullFractalCandlestick?.LowPrice
                    && currentBearFractalCandlestick?.HighPrice > previousBearFractalCandlestick?.HighPrice)
                {
                    candlestick.FractalTrend = Trend.Up;
                    continue;
                }

                if (currentBullFractalCandlestick?.LowPrice < previousBullFractalCandlestick?.LowPrice
                    && currentBearFractalCandlestick?.HighPrice < previousBearFractalCandlestick?.HighPrice)
                {
                    candlestick.FractalTrend = Trend.Down;
                    continue;
                }

                if (currentBullFractalCandlestick?.LowPrice >= previousBullFractalCandlestick?.LowPrice
                    && currentBearFractalCandlestick?.HighPrice <= previousBearFractalCandlestick?.HighPrice)
                {
                    candlestick.FractalTrend = Trend.Sideways;
                    continue;
                }

                if (currentBullFractalCandlestick?.LowPrice <= previousBullFractalCandlestick?.LowPrice
                    && currentBearFractalCandlestick?.HighPrice >= previousBearFractalCandlestick?.HighPrice)
                {
                    candlestick.FractalTrend = Trend.Sideways;
                }
            }
        }

        private static void CalculatePriceTrendBasedOnFractalPattern(PairExtended pair)
        {
            CandlestickExtended? currentBullFractalCandlestick = null;
            CandlestickExtended? currentBearFractalCandlestick = null;

            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick3 = i - 2 >= 0 ? pair.Candlesticks[i - 2] : null;

                if (candlestick3 is null)
                {
                    continue;
                }

                if (candlestick3.Fractals.Exists(f => f.FractalType == FractalType.BullFractal
                                                              && f.WindowPeriod == 2))
                {
                    if (currentBullFractalCandlestick is null || candlestick3.PrimaryId > currentBullFractalCandlestick?.PrimaryId)
                    {
                        currentBullFractalCandlestick = candlestick3;
                    }
                }

                if (candlestick3.Fractals.Exists(f => f.FractalType == FractalType.BearFractal
                                                              && f.WindowPeriod == 2))
                {
                    if (currentBearFractalCandlestick is null || candlestick3.PrimaryId > currentBearFractalCandlestick.PrimaryId)
                    {
                        currentBearFractalCandlestick = candlestick3;
                    }
                }

                if (currentBullFractalCandlestick is null
                    || currentBearFractalCandlestick is null)
                {
                    continue;
                }

                if (candlestick.HighPrice > currentBearFractalCandlestick.HighPrice
                    && candlestick.LowPrice > currentBullFractalCandlestick.LowPrice)
                {
                    candlestick.PriceTrend = Trend.Up;
                    continue;
                }

                if (candlestick.LowPrice < currentBullFractalCandlestick.LowPrice
                    && candlestick.HighPrice < currentBearFractalCandlestick.HighPrice)
                {
                    candlestick.PriceTrend = Trend.Down;
                }

                if (candlestick.LowPrice > currentBullFractalCandlestick.LowPrice
                   && candlestick.HighPrice < currentBearFractalCandlestick.HighPrice)
                {
                    candlestick.PriceTrend = Trend.Sideways;
                }

                if (candlestick.LowPrice < currentBullFractalCandlestick.LowPrice
                    && candlestick.HighPrice > currentBearFractalCandlestick.HighPrice)
                {
                    candlestick.PriceTrend = Trend.Sideways;
                }
            }
        }

        private static void CalculateResistanceBreakout(PairExtended pair)
        {
            const int initialInvestment = 1000;
            for (int i = 0; i < pair?.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = i - 1 >= 0 ? pair.Candlesticks[i - 1] : null;

                if (candlestick1?.FlagsNestedCandlesticksBody == null)
                {
                    continue;
                }

                foreach (var flagNestedCandlestickBody in candlestick1.FlagsNestedCandlesticksBody.Where(f => f.NumberOfNestedCandlestickBodies > 3))
                {
                    var candlestickFlagPole = pair.Candlesticks.Find(c => c.PrimaryId == flagNestedCandlestickBody.FlagPoleCandlestickId);

                    if (candlestick.ClosePrice > candlestickFlagPole?.HighPrice)
                    {
                        candlestick.ResistanceBreakouts.Add(new ResistanceBreakout(candlestick.PrimaryId)
                        {
                            OrderOfSignal = 1,
                            IsBuy = true,
                            FlagPoleCandlestickId = candlestickFlagPole.PrimaryId,
                            PurchaseAmount = initialInvestment / candlestick.ClosePrice
                        });
                    }
                }
            }

            CalculateResistanceBreakoutTakeProfitStopLoss(pair);
        }

        private static void CalculateResistanceBreakoutTakeProfitStopLoss(PairExtended pair)
        {
            if (pair?.Candlesticks.Count > 0)
            {
                for (int i = 1; i < pair.Candlesticks.Count; i++)
                {
                    var signalCandlestick = pair.Candlesticks[i];

                    if (signalCandlestick.ResistanceBreakouts.Count > 0)
                    {
                        for (int j = i; j < pair.Candlesticks.Count; j++)
                        {
                            var candlestick = pair.Candlesticks[j];

                            if (candlestick.ClosePrice >= signalCandlestick.ClosePrice + (signalCandlestick.Body * 2)
                                && !signalCandlestick.ResistanceBreakouts[0].IsPositionClosed)
                            {
                                signalCandlestick.ResistanceBreakouts[0].ClosePosition = true;
                                signalCandlestick.ResistanceBreakouts[0].ExitPositionDate = candlestick.CloseDate;
                                signalCandlestick.ResistanceBreakouts[0].IsPositionClosed = true;
                                signalCandlestick.ResistanceBreakouts[0].ProfitInPoints = candlestick.ClosePrice - signalCandlestick.ClosePrice;
                                signalCandlestick.ResistanceBreakouts[0].ValueAtExit = signalCandlestick.ResistanceBreakouts[0].PurchaseAmount * candlestick.ClosePrice;
                                signalCandlestick.ResistanceBreakouts[0].ProfitInMoney = signalCandlestick.ResistanceBreakouts[0].ValueAtExit - 1000;
                                break;
                            }

                            if (candlestick.ClosePrice <= signalCandlestick.ClosePrice - signalCandlestick.Body
                                && !signalCandlestick.ResistanceBreakouts[0].IsPositionClosed
                            )
                            {
                                signalCandlestick.ResistanceBreakouts[0].ClosePosition = true;
                                signalCandlestick.ResistanceBreakouts[0].ExitPositionDate = candlestick.CloseDate;
                                signalCandlestick.ResistanceBreakouts[0].IsPositionClosed = true;
                                signalCandlestick.ResistanceBreakouts[0].LossInPoints = candlestick.ClosePrice - signalCandlestick.ClosePrice;
                                signalCandlestick.ResistanceBreakouts[0].ValueAtExit = signalCandlestick.ResistanceBreakouts[0].PurchaseAmount * candlestick.ClosePrice;
                                signalCandlestick.ResistanceBreakouts[0].LossInMoney = signalCandlestick.ResistanceBreakouts[0].ValueAtExit - 1000;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static void CalculateEnchancedLong(PairExtended pair,
            Dictionary<DateTime, FearAndGreedModel> cryptoFearAndGreedDataPerDatetime,
            Dictionary<DateTime, FearAndGreedModel> stockFearAndGreedDataPerDatetime)
        {
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];

                //TODO Enable it debug specific candlestick
                if (candlestick.CloseDate.Date == new DateTime(2024, 04, 23).Date
                    && string.Equals(pair.Symbol, "AAPL", StringComparison.InvariantCultureIgnoreCase))
                {
                }

                if (IsAscendingGreenCandlestickPattern(pair.Candlesticks, i))
                {
                    // continue;
                }

                if (cryptoFearAndGreedDataPerDatetime.TryGetValue(candlestick.CloseDate.Date, out var cryptoFearAndGreedIndex)
                    && cryptoFearAndGreedIndex is not null && pair.Provider
                    is DataProvider.Binance
                    or DataProvider.Uniswap
                    or DataProvider.Pancakeswap)
                {
                    var greedAndFearCondition = cryptoFearAndGreedIndex.ValueClassificationType is ValueClassificationType.ExtremeFear
                          or ValueClassificationType.Fear
                          or ValueClassificationType.Neutral;

                    if (!greedAndFearCondition)
                    {
                        continue;
                    }
                }

                if (stockFearAndGreedDataPerDatetime.TryGetValue(candlestick.CloseDate.Date, out var stockFearAndGreedIndex)
                     && stockFearAndGreedIndex is not null && pair.Provider
                     is DataProvider.Alpaca)
                {
                    var greedAndFearCondition = stockFearAndGreedIndex.ValueClassificationType is ValueClassificationType.ExtremeFear
                          or ValueClassificationType.Fear
                          or ValueClassificationType.Neutral;

                    if (!greedAndFearCondition)
                    {
                        continue;
                    }
                }

                bool[] conditions =
                [
                    GetOversoldBollingerConditions(pair.Candlesticks, i),
                    GetOversoldDonchianConditions(pair.Candlesticks, i),
                    GetOversoldKeltnerConditions(pair.Candlesticks, i),
                    GetLowestLowCondition(pair.Candlesticks, i),
                    GetOversoldRsiConditions(pair.Candlesticks, i),
                    GetOversoldStochasticConditions(pair.Candlesticks, i),
                    GetOversoldCciConditions(pair.Candlesticks, i),
                    GetOversoldAdxConditions(pair.Candlesticks, i),
                    GetOversoldAroonConditions(pair.Candlesticks, i),
                    GetFractalBullCondition(pair.Candlesticks, i),
                    GetHighestHighVixFix(pair.Candlesticks, i),
                    GetPivotSupportOversoldCondition(pair.Candlesticks, i),
                    GetFractalEnhancedLongTrend(pair.Candlesticks, i),
                    GetPriceEnhancedLongTrend(pair.Candlesticks, i),
                    GetOnBalanceVolumeCondition(pair.Candlesticks, i),
                    GetPsychologicalLineCondition(pair.Candlesticks, i),
                    // GetVolumeCondition(pair.Candlesticks, i)
                    // GetOversoldMacdConditions(pair.Candlesticks, i), //It is bad for signal
                    // GetOversoldRateOfChange(pair.Candlesticks, i) //It is bad for signal
                ];

                var consesutiveCandlesticksBelowSma = GetConsecutiveCandlesticksBelowSmaCondition(pair, i);
                if (consesutiveCandlesticksBelowSma)
                {
                    //  conditions = [.. conditions, consesutiveCandlesticksBelowSma];
                }

                int trueConditionsCount = conditions.Count(condition => condition);
                double percentageTrueConditions = (double)trueConditionsCount / conditions.Length * 100;

                const int threshold = 90;

                //Try dynamic percentage based on volatility
                /* if (candlestick.Volatilities.FirstOrDefault().VolatilityValue < 50)
                 {
                     threshold = 85;
                 }
                 else if (candlestick.Volatilities.FirstOrDefault().VolatilityValue >= 50)
                 {
                     threshold = 90;
                 }*/

                if (percentageTrueConditions >= threshold
                // TODO test it with candlesticks patterns
                /*                                &&
                                                (
                                                    candlestick?.DragonFlyDojis.FirstOrDefault()?.IsDragonFlyDoji == true
                                                    || candlestick?.Hammers.FirstOrDefault()?.IsHammer == true
                                                    || candlestick?.InvertedHammers.FirstOrDefault()?.IsInvertedHammer == true
                                                    || candlestick?.SpinningTops.FirstOrDefault()?.IsSpinningTop == true
                                                    || candlestick?.Marubozus.FirstOrDefault()?.IsMarubozu == true
                                                    || candlestick?.PriceFunnels.FirstOrDefault()?.IsFunnel == true
                                                    || candlestick?.TypicalPriceReversals.FirstOrDefault()?.TypicalPriceReversalIsBuy == true
                                                )*/
                )
                {
                    var candlestick1 = i >= 1 ? pair.Candlesticks[i - 1] : null;
                    var firstEnhancedScan = candlestick1?.EnhancedScans.FirstOrDefault();

                    if (firstEnhancedScan?.EnhancedScanIsLong == true)
                    {
                        candlestick.EnhancedScans.Add(new EnhancedScan(candlestick.PrimaryId)
                        {
                            EnhancedScanIsLong = true,
                            OrderOfLongSignal = firstEnhancedScan.OrderOfLongSignal + 1
                        });
                    }
                    else
                    {
                        candlestick.EnhancedScans.Add(new EnhancedScan(candlestick.PrimaryId)
                        {
                            EnhancedScanIsLong = true,
                            OrderOfLongSignal = 1
                        });
                    }
                }
            }
        }

        private static void CalculateEnchancedShort(PairExtended pair,
            Dictionary<DateTime, FearAndGreedModel> cryptoFearAndGreedDataPerDatetime,
            Dictionary<DateTime, FearAndGreedModel> stockFearAndGreedDataPerDatetime)
        {
            decimal? candlestickHighestPrice = -1;

            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];

                if (cryptoFearAndGreedDataPerDatetime.TryGetValue(candlestick.CloseDate.Date, out var cryptoFearAndGreedIndex)
                    && cryptoFearAndGreedIndex is not null && pair.Provider
                    is DataProvider.Binance
                    or DataProvider.Uniswap
                    or DataProvider.Pancakeswap)
                {
                    var greedAndFearCondition = cryptoFearAndGreedIndex.ValueClassificationType is ValueClassificationType.Greed
                          or ValueClassificationType.ExtremeGreed;

                    if (!greedAndFearCondition)
                    {
                        continue;
                    }
                }

                if (stockFearAndGreedDataPerDatetime.TryGetValue(candlestick.CloseDate.Date, out var stockFearAndGreedIndex)
                    && stockFearAndGreedIndex is not null && pair.Provider
                    is DataProvider.Alpaca)
                {
                    var greedAndFearCondition = stockFearAndGreedIndex.ValueClassificationType is ValueClassificationType.Greed
                          or ValueClassificationType.ExtremeGreed;

                    if (!greedAndFearCondition)
                    {
                        continue;
                    }
                }

                if (candlestick.HighPrice > candlestickHighestPrice)
                {
                    candlestickHighestPrice = candlestick.HighPrice;
                }

                if (!GetHighestHighCandlestickConditions(pair.Candlesticks, i, candlestickHighestPrice))
                {
                    continue;
                }

                if (pair.Symbol == "CREAM-USDT" && candlestick.CloseDate.Date == new DateTime(2024, 03, 29))
                {
                }

                bool[] conditions =
                [
                    GetOverboughtRsiConditions(pair.Candlesticks, i),
                    GetOverboughtStochasticConditions(pair.Candlesticks, i),
                    GetOverboughtAdxConditions(pair.Candlesticks, i),
                    GetOverboughtCciConditions(pair.Candlesticks, i),
                    GetOverboughtBollingerConditions(pair.Candlesticks, i),
                    GetOverboughtKeltnerConditions(pair.Candlesticks, i),
                    GetOverboughtAroonConditions(pair.Candlesticks, i),
                    GetOverboughtDonchianConditions(pair.Candlesticks, i),
                    GetPivotSupportOverboughtConditions(pair.Candlesticks, i),
                    GetFractalBearCondition(pair.Candlesticks, i),
                    GetFractalEnhancedShortTrend(pair.Candlesticks, i),
                    GetPriceEnhancedShortTrend(pair.Candlesticks, i),
                ];

                int trueConditionsCount = conditions.Count(condition => condition);
                double percentageTrueConditions = (double)trueConditionsCount / conditions.Length * 100;

                const int threshold = 90;

                if (percentageTrueConditions >= threshold)
                {
                    var candlestick1 = i - 1 >= 0 ? pair.Candlesticks[i - 1] : null;
                    var firstEnhancedScan = candlestick1?.EnhancedScans.FirstOrDefault();

                    if (firstEnhancedScan?.EnhancedScanIsShort == true)
                    {
                        candlestick.EnhancedScans.Add(new EnhancedScan(candlestick.PrimaryId)
                        {
                            EnhancedScanIsShort = true,
                            OrderOfShortSignal = firstEnhancedScan.OrderOfShortSignal + 1
                        });
                    }
                    else
                    {
                        candlestick.EnhancedScans.Add(new EnhancedScan(candlestick.PrimaryId)
                        {
                            EnhancedScanIsShort = true,
                            OrderOfShortSignal = 1
                        });
                    }
                }
            }
        }

        private static bool GetConsecutiveCandlesticksBelowSmaCondition(PairExtended pair, int index)
        {
            if (index <= 0 || index - 1 <= 0 || index - 2 <= 0)
            {
                return false;
            }

            return pair.Candlesticks[index]?.ConsecutiveCandlesticksBelowSma >= 5
                || pair.Candlesticks[index - 1]?.ConsecutiveCandlesticksBelowSma >= 5
                || pair.Candlesticks[index - 2]?.ConsecutiveCandlesticksBelowSma >= 5;
        }

        private static void CalculateVerticalHorizontalFilterRange(PairExtended pair)
        {
            int count = 0;
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                CandlestickExtended currentCandlestick = pair.Candlesticks[i];

                var verticalHorizontalFilter = currentCandlestick.VerticalHorizontalFilters?.FirstOrDefault();
                if (verticalHorizontalFilter?.Value is 0)
                {
                    continue;
                }

                //TODO Enable it debug specific candlestick
                if (currentCandlestick.CloseDate.Date == new DateTime(2023, 12, 8).Date
                    && string.Equals(pair.BaseAssetName, "JTO", StringComparison.InvariantCultureIgnoreCase))
                {
                }

                bool hasVertialHorizontalFilterRange = verticalHorizontalFilter?.Value <= Constants.VerticalHorizontalFilterRangeLimit;
                bool hasAdxRange = currentCandlestick?.Adxs?.FirstOrDefault()?.PlusDi <= 25
                    && currentCandlestick?.Adxs?.FirstOrDefault()?.MinusDi <= 25
                    && currentCandlestick?.Adxs?.FirstOrDefault()?.Value <= 25;

                bool hasStochasticRange = currentCandlestick?.Stochastics?.FirstOrDefault()?.SignalD <= 80
                    && currentCandlestick?.Stochastics?.FirstOrDefault()?.OscillatorK <= 25
                    && currentCandlestick?.Stochastics?.FirstOrDefault()?.SignalD >= 20
                    && currentCandlestick?.Stochastics?.FirstOrDefault()?.OscillatorK >= 20;

                bool hasRsiRange = currentCandlestick?.Rsis?.FirstOrDefault()?.Value is <= 70 and
                    >= 30;

                //TODO Calculate vixfix under a specific level <-- new strategy, just added it here as note
                bool hasBollingerBandsRange = currentCandlestick?.ClosePrice <= currentCandlestick?.BollingerBands?.FirstOrDefault()?.UpperBand
                    && currentCandlestick?.ClosePrice >= currentCandlestick?.BollingerBands?.FirstOrDefault()?.LowerBand;

                if (hasVertialHorizontalFilterRange && hasRsiRange)
                {
                    count++;

                    var verticalHorizontalFilterRange = new VerticalHorizontalFilterRange(currentCandlestick.PrimaryId)
                    {
                        NumberOfCandlesticksInRange = count,
                        RangeLimit = Constants.VerticalHorizontalFilterRangeLimit
                    };
                    currentCandlestick.VerticalHorizontalFilterRanges.Add(verticalHorizontalFilterRange);
                }
                else
                {
                    count = 0;
                }
            }
        }

        private static bool IsAscendingGreenCandlestickPattern(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 2 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var currentCandlestick = candlesticks[currentIndex];
            var candlestick1 = candlesticks[currentIndex - 1];
            var candlestick2 = candlesticks[currentIndex - 2];

            bool isCurrentCandlestickBullish = currentCandlestick.ClosePrice > candlestick1.OpenPrice;
            bool isCandlestick1Bullish = candlestick1.ClosePrice > candlestick1.OpenPrice;
            bool isCandlestick2Bullish = candlestick2.ClosePrice > candlestick2.OpenPrice;

            bool areThreeLatestCandlesticksBullish = isCurrentCandlestickBullish && isCandlestick1Bullish && isCandlestick2Bullish;

            bool isHigherHighCurrent = currentCandlestick.HighPrice > candlestick1.HighPrice;
            bool isHigherHighPrevious = candlestick1.HighPrice > candlestick2.HighPrice;

            bool isHigherLowCurrent = currentCandlestick.LowPrice > candlestick1.LowPrice;
            bool isHigherLowPrevious = candlestick1.LowPrice > candlestick2.LowPrice;

            bool isValidWhiteSoldiersPattern = isHigherHighCurrent && isHigherHighPrevious && isHigherLowCurrent && isHigherLowPrevious;

            return areThreeLatestCandlesticksBullish && isValidWhiteSoldiersPattern;
        }

        private static void CalculateCandlestickPatterns(PairExtended pair)
        {
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = i - 1 >= 0 ? pair.Candlesticks[i - 1] : null;
                var candlestick2 = i - 2 >= 0 ? pair.Candlesticks[i - 2] : null;

                CalculateTypicalPriceReversal(candlestick, candlestick1, candlestick2);
                CalculateDragonflyDoji(candlestick);
                CalculateHammer(candlestick);
                CalculateMarubozu(candlestick);
                CalculateInvertedHammer(candlestick);
                CalculateSpinningTop(candlestick);
            }
        }

        private static void CalculateDragonflyDoji(CandlestickExtended candlestick)
        {
            if (
                candlestick.Range >= 2 * candlestick.Body
                && candlestick.ClosePrice >= candlestick.HighPrice - (candlestick.Range * 0.5m)
                && candlestick.OpenPrice >= candlestick.HighPrice - (candlestick.Range * 0.5m)
                && candlestick.BodyToRangeRatio <= Constants.BodyOneToTenOfRangeRatio
                && candlestick.OpenPrice >= candlestick.TopTwentyPercentOfRangeInPriceUnit
                && candlestick.ClosePrice >= candlestick.TopTwentyPercentOfRangeInPriceUnit
                //TODO test with range >= atr * Multiplier
                )
            {
                candlestick.DragonFlyDojis.Add(
                    new DragonFlyDoji(candlestick.PrimaryId)
                    {
                        IsDragonFlyDoji = true
                    });
            }
        }

        private static void CalculateMarubozu(CandlestickExtended candlestick)
        {
            if (candlestick.Body >= candlestick.Range * 0.9m)
            {
                candlestick.Marubozus.Add(
                    new Marubozu(candlestick.PrimaryId) { IsMarubozu = true });
            }
        }

        private static void CalculateHammer(CandlestickExtended candlestick)
        {
            if (
                candlestick.Range > 0 // To avoid divide by zero
                && candlestick.Range >= 2 * candlestick.Body
                && (candlestick.ClosePrice - candlestick.LowPrice) / candlestick.Range > 0.5m
                && (candlestick.OpenPrice - candlestick.LowPrice) / candlestick.Range > 0.5m
                )
            {
                candlestick.Hammers.Add(
                    new Hammer(candlestick.PrimaryId) { IsHammer = true });
            }
        }

        private static void CalculateInvertedHammer(CandlestickExtended candlestick)
        {
            if (candlestick.Range > 0 // To avoid divide by zero
                 && candlestick.Range >= 2 * candlestick.Body
                 && (candlestick.HighPrice - candlestick.ClosePrice) / candlestick.Range > 0.5m //Close price position higher that mid
                 && (candlestick.HighPrice - candlestick.OpenPrice) / candlestick.Range > 0.5m //Open price position higher that mid
                 && candlestick.OpenPrice <= candlestick.BottomTwentyPercentOfRangeInPriceUnit
                 && candlestick.ClosePrice <= candlestick.BottomTwentyPercentOfRangeInPriceUnit)
            {
                candlestick.InvertedHammers.Add(
                    new InvertedHammer(candlestick.PrimaryId) { IsInvertedHammer = true });
            }
        }

        private static void CalculateSpinningTop(CandlestickExtended candlestick)
        {
            if (
                candlestick.Range >= 2 * candlestick.Body
                && candlestick.OpenPrice <= candlestick.TenPercentHigherThanMidRangeInPriceUnit
                && candlestick.ClosePrice <= candlestick.TenPercentHigherThanMidRangeInPriceUnit
                && candlestick.OpenPrice >= candlestick.TenPercentLowerThanMidRangeInPriceUnit
                && candlestick.ClosePrice >= candlestick.TenPercentLowerThanMidRangeInPriceUnit
                )
            {
                candlestick.SpinningTops.Add(
                    new SpinningTop(candlestick.PrimaryId) { IsSpinningTop = true });
            }
        }

        // https://www.tradingsetupsreview.com/two-bar-reversal-pattern-trading-guide/
        private static void CalculateTypicalPriceReversal(CandlestickExtended candlestick, CandlestickExtended? candlestick1, CandlestickExtended? candlestick2)
        {
            if (
                 candlestick1?.TypicalPrice <= candlestick2?.LowPrice &&
                 candlestick1.Body >= candlestick1.Range * 0.5m &&
                 candlestick1.ClosePrice <= candlestick1.OpenPrice &&
                 candlestick.Body >= candlestick.Range * 0.5m &&
                 candlestick.ClosePrice >= candlestick.OpenPrice &&
                 candlestick1.TypicalPrice <= candlestick.HighPrice &&
                 candlestick1.TypicalPrice >= candlestick.LowPrice &&
                 candlestick.TypicalPrice <= candlestick1.HighPrice &&
                 candlestick.TypicalPrice >= candlestick1.LowPrice
                )
            {
                candlestick.TypicalPriceReversals.Add(new TypicalPriceReversal(candlestick.PrimaryId)
                {
                    OrderOfSignal = 1,
                    TypicalPriceReversalIsBuy = true,
                    TypicalPriceReversalIsSell = false
                });
            }
        }

        private static bool GetLowestLowCondition(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var lowestLow = candlesticks[currentIndex].Lowests.FirstOrDefault();
            var lowestLow1 = currentIndex - 1 >= 0 && currentIndex - 1 < candlesticks.Count ? candlesticks[currentIndex - 1].Lowests.Find(c => c.PriceType == PriceType.Low && c.Period == 5) : null;
            var lowestLow2 = currentIndex - 2 >= 0 && currentIndex - 2 < candlesticks.Count ? candlesticks[currentIndex - 2].Lowests.Find(c => c.PriceType == PriceType.Low && c.Period == 5) : null;
            var lowestLow3 = currentIndex - 3 >= 0 && currentIndex - 3 < candlesticks.Count ? candlesticks[currentIndex - 3].Lowests.Find(c => c.PriceType == PriceType.Low && c.Period == 5) : null;
            var lowestLow4 = currentIndex - 4 >= 0 && currentIndex - 4 < candlesticks.Count ? candlesticks[currentIndex - 4].Lowests.Find(c => c.PriceType == PriceType.Low && c.Period == 5) : null;

            return candlesticks[currentIndex].LowPrice <= lowestLow?.Value
                || candlesticks[currentIndex - 1].LowPrice <= lowestLow1?.Value
                || candlesticks[currentIndex - 2].LowPrice <= lowestLow2?.Value
                || candlesticks[currentIndex - 3].LowPrice <= lowestLow3?.Value
                || candlesticks[currentIndex - 4].LowPrice <= lowestLow4?.Value;
        }

        private static bool GetHighestHighVixFix(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var highestHighVixFix = candlesticks[currentIndex].Highests.Find(c => c.PriceType == PriceType.VixFix && c.Period == 5);

            var highestHighVixFix1 = currentIndex - 1 >= 0 && currentIndex - 1 < candlesticks.Count
                ? candlesticks[currentIndex - 1].Highests.Find(c => c.PriceType == PriceType.VixFix && c.Period == 5)
                : null;

            var highestHighVixFix2 = currentIndex - 2 >= 0 && currentIndex - 2 < candlesticks.Count
                ? candlesticks[currentIndex - 2].Highests.Find(c => c.PriceType == PriceType.VixFix && c.Period == 5)
                : null;

            var highestHighVixFix3 = currentIndex - 3 >= 0 && currentIndex - 3 < candlesticks.Count
                ? candlesticks[currentIndex - 3].Highests.Find(c => c.PriceType == PriceType.VixFix && c.Period == 5)
                : null;

            var highestHighVixFix4 = currentIndex - 4 >= 0 && currentIndex - 4 < candlesticks.Count
                ? candlesticks[currentIndex - 4].Highests.Find(c => c.PriceType == PriceType.VixFix && c.Period == 5)
                : null;

            return candlesticks[currentIndex].VixFixes.FirstOrDefault()?.Value <= highestHighVixFix?.Value
                || (highestHighVixFix1 is not null && candlesticks[currentIndex - 1].VixFixes.FirstOrDefault()?.Value <= highestHighVixFix1.Value)
                || (highestHighVixFix2 is not null && candlesticks[currentIndex - 2].VixFixes.FirstOrDefault()?.Value <= highestHighVixFix2.Value)
                || (highestHighVixFix3 is not null && candlesticks[currentIndex - 3].VixFixes.FirstOrDefault()?.Value <= highestHighVixFix3.Value)
                || (highestHighVixFix4 is not null && candlesticks[currentIndex - 4].VixFixes.FirstOrDefault()?.Value <= highestHighVixFix4.Value);
        }

        private static bool GetFractalEnhancedLongTrend(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var candlestick = candlesticks[currentIndex];
            var candlestick1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1] : null;
            var candlestick2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2] : null;
            var candlestick3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3] : null;
            var candlestick4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4] : null;

            if (candlestick is null ||
                candlestick1 is null ||
                candlestick2 is null ||
                candlestick3 is null ||
                candlestick4 is null)
            {
                return false;
            }

            return candlestick.FractalTrend is Trend.Down ||
                candlestick1.FractalTrend is Trend.Down ||
                candlestick2.FractalTrend is Trend.Down ||
                candlestick3.FractalTrend is Trend.Down ||
                candlestick4.FractalTrend is Trend.Down ||
                candlestick.FractalTrend is Trend.Sideways ||
                candlestick1.FractalTrend is Trend.Sideways ||
                candlestick2.FractalTrend is Trend.Sideways ||
                candlestick3.FractalTrend is Trend.Sideways ||
                candlestick4.FractalTrend is Trend.Sideways;
        }

        private static bool GetPriceEnhancedLongTrend(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var candlestick = candlesticks[currentIndex];
            var candlestick1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1] : null;
            var candlestick2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2] : null;
            var candlestick3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3] : null;
            var candlestick4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4] : null;

            if (candlestick is null ||
                candlestick1 is null ||
                candlestick2 is null ||
                candlestick3 is null ||
                candlestick4 is null)
            {
                return false;
            }

            return candlestick.PriceTrend is Trend.Down ||
                candlestick1.PriceTrend is Trend.Down ||
                candlestick2.PriceTrend is Trend.Down ||
                candlestick3.PriceTrend is Trend.Down ||
                candlestick4.PriceTrend is Trend.Down;
        }

        private static bool GetFractalBullCondition(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var candlestick2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2] : null;
            var candlestick3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3] : null;
            var candlestick4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4] : null;

            if (candlestick2 is null ||
                candlestick3 is null ||
                candlestick4 is null)
            {
                return false;
            }

            return candlestick2.Fractals.Find(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2)?.Value.HasValue == true ||
                   candlestick3.Fractals.Find(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2)?.Value.HasValue == true ||
                   candlestick4.Fractals.Find(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2)?.Value.HasValue == true;
        }

        private static bool GetOversoldRsiConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var rsi = candlesticks[currentIndex].Rsis.FirstOrDefault();
            var rsi1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Rsis?.FirstOrDefault() : null;
            var rsi2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Rsis?.FirstOrDefault() : null;
            var rsi3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Rsis?.FirstOrDefault() : null;
            var rsi4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Rsis?.FirstOrDefault() : null;

            return rsi?.Value <= Constants.RsiOversold ||
                rsi1?.Value <= Constants.RsiOversold ||
                rsi2?.Value <= Constants.RsiOversold ||
                rsi3?.Value <= Constants.RsiOversold ||
                rsi4?.Value <= Constants.RsiOversold;
        }

        private static bool GetOversoldStochasticConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var stochastic = candlesticks[currentIndex].Stochastics.FirstOrDefault();
            var stochastic1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Stochastics?.FirstOrDefault() : null;
            var stochastic2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Stochastics?.FirstOrDefault() : null;
            var stochastic3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Stochastics?.FirstOrDefault() : null;
            var stochastic4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Stochastics?.FirstOrDefault() : null;

            return stochastic?.OscillatorK <= Constants.StochasticOversold ||
                stochastic1?.OscillatorK <= Constants.StochasticOversold ||
                stochastic2?.OscillatorK <= Constants.StochasticOversold ||
                stochastic3?.OscillatorK <= Constants.StochasticOversold ||
                stochastic4?.OscillatorK <= Constants.StochasticOversold ||
                stochastic?.SignalD <= Constants.StochasticOversold ||
                stochastic1?.SignalD <= Constants.StochasticOversold ||
                stochastic2?.SignalD <= Constants.StochasticOversold ||
                stochastic3?.SignalD <= Constants.StochasticOversold ||
                stochastic4?.SignalD <= Constants.StochasticOversold;
        }

        private static bool GetOversoldAdxConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var adx = candlesticks[currentIndex].Adxs.FirstOrDefault();
            var adx1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Adxs?.FirstOrDefault() : null;
            var adx2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Adxs?.FirstOrDefault() : null;
            var adx3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Adxs?.FirstOrDefault() : null;
            var adx4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Adxs?.FirstOrDefault() : null;

            return adx?.PlusDi <= Constants.AdxOversold ||
                adx1?.PlusDi <= Constants.AdxOversold ||
                adx2?.PlusDi <= Constants.AdxOversold ||
                adx3?.PlusDi <= Constants.AdxOversold ||
                adx4?.PlusDi <= Constants.AdxOversold;
        }

        private static bool GetOversoldBollingerConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var bollingerBand = candlesticks[currentIndex].BollingerBands.FirstOrDefault();
            var bollingerBand1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].BollingerBands.FirstOrDefault() : null;
            var bollingerBand2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].BollingerBands.FirstOrDefault() : null;
            var bollingerBand3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].BollingerBands.FirstOrDefault() : null;
            var bollingerBand4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].BollingerBands.FirstOrDefault() : null;

            if (bollingerBand == null
                || bollingerBand1 == null
                || bollingerBand2 == null
                || bollingerBand3 == null
                || bollingerBand4 == null)
            {
                return false;
            }

            if (!decimal.TryParse(bollingerBand.LowerBand.ToString(), out decimal _)
                || !decimal.TryParse(bollingerBand1.LowerBand.ToString(), out decimal _)
                || !decimal.TryParse(bollingerBand2.LowerBand.ToString(), out decimal _)
                || !decimal.TryParse(bollingerBand3.LowerBand.ToString(), out decimal _)
                || !decimal.TryParse(bollingerBand4.LowerBand.ToString(), out decimal _))
            {
                return false;
            }

            return candlesticks[currentIndex].LowPrice <= bollingerBand.LowerBand
                || candlesticks[currentIndex - 1].LowPrice <= bollingerBand1.LowerBand
                || candlesticks[currentIndex - 2].LowPrice <= bollingerBand2.LowerBand
                || candlesticks[currentIndex - 3].LowPrice <= bollingerBand3.LowerBand
                || candlesticks[currentIndex - 4].LowPrice <= bollingerBand4.LowerBand;
        }

        private static bool GetOversoldKeltnerConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var keltnerChannel = candlesticks[currentIndex].KeltnerChannels.FirstOrDefault();
            var keltnerChannel1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].KeltnerChannels.FirstOrDefault() : null;
            var keltnerChannel2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].KeltnerChannels.FirstOrDefault() : null;
            var keltnerChannel3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].KeltnerChannels.FirstOrDefault() : null;
            var keltnerChannel4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].KeltnerChannels.FirstOrDefault() : null;

            if (keltnerChannel == null || keltnerChannel1 == null || keltnerChannel2 == null || keltnerChannel3 == null || keltnerChannel4 == null)
            {
                return false;
            }

            if (!decimal.TryParse(keltnerChannel.LowerBand.ToString(), out decimal _)
             || !decimal.TryParse(keltnerChannel1.LowerBand.ToString(), out decimal _)
             || !decimal.TryParse(keltnerChannel2.LowerBand.ToString(), out decimal _)
             || !decimal.TryParse(keltnerChannel3.LowerBand.ToString(), out decimal _)
             || !decimal.TryParse(keltnerChannel4.LowerBand.ToString(), out decimal _))
            {
                return false;
            }

            return candlesticks[currentIndex].LowPrice <= (decimal)keltnerChannel.LowerBand
                || candlesticks[currentIndex - 1].LowPrice <= (decimal)keltnerChannel1.LowerBand
                || candlesticks[currentIndex - 2].LowPrice <= (decimal)keltnerChannel2.LowerBand
                || candlesticks[currentIndex - 3].LowPrice <= (decimal)keltnerChannel3.LowerBand
                || candlesticks[currentIndex - 4].LowPrice <= (decimal)keltnerChannel4.LowerBand;
        }

        private static bool GetOversoldDonchianConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var donchianChannel = candlesticks[currentIndex].DonchianChannels.FirstOrDefault();
            var donchianChannel1 = currentIndex >= 1 ? candlesticks[currentIndex - 1].DonchianChannels.FirstOrDefault() : null;
            var donchianChannel2 = currentIndex >= 2 ? candlesticks[currentIndex - 2].DonchianChannels.FirstOrDefault() : null;
            var donchianChannel3 = currentIndex >= 3 ? candlesticks[currentIndex - 3].DonchianChannels.FirstOrDefault() : null;
            var donchianChannel4 = currentIndex >= 4 ? candlesticks[currentIndex - 4].DonchianChannels.FirstOrDefault() : null;

            if (donchianChannel == null || donchianChannel1 == null || donchianChannel2 == null || donchianChannel3 == null || donchianChannel4 == null)
            {
                return false;
            }

            return (candlesticks[currentIndex]?.LowPrice != null && donchianChannel.LowerBand is not null && candlesticks[currentIndex].LowPrice <= (decimal)donchianChannel.LowerBand)
                || (candlesticks[currentIndex - 1]?.LowPrice is not null && donchianChannel1.LowerBand != null && candlesticks[currentIndex - 1].LowPrice <= (decimal)donchianChannel1.LowerBand)
                || (candlesticks[currentIndex - 2]?.LowPrice is not null && donchianChannel2.LowerBand != null && candlesticks[currentIndex - 2].LowPrice <= (decimal)donchianChannel2.LowerBand)
                || (candlesticks[currentIndex - 3]?.LowPrice != null && donchianChannel3.LowerBand is not null && candlesticks[currentIndex - 3].LowPrice <= (decimal)donchianChannel3.LowerBand)
                || (candlesticks[currentIndex - 4]?.LowPrice != null && donchianChannel4.LowerBand is not null && candlesticks[currentIndex - 4].LowPrice <= (decimal)donchianChannel4.LowerBand);
        }

        private static bool GetOversoldAroonConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var aroon = candlesticks[currentIndex].Aroons.FirstOrDefault();
            var aroon1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Aroons.FirstOrDefault() : null;
            var aroon2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Aroons.FirstOrDefault() : null;
            var aroon3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Aroons.FirstOrDefault() : null;
            var aroon4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Aroons.FirstOrDefault() : null;

            return aroon?.AroonDown >= 80
                || aroon1?.AroonDown >= 80
                || aroon2?.AroonDown >= 80
                || aroon3?.AroonDown >= 80
                || aroon4?.AroonDown >= 80;
        }

        private static bool GetOversoldCciConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var cci = candlesticks[currentIndex].Ccis.FirstOrDefault();
            var cci1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Ccis.FirstOrDefault() : null;
            var cci2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Ccis.FirstOrDefault() : null;
            var cci3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Ccis.FirstOrDefault() : null;
            var cci4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Ccis.FirstOrDefault() : null;

            if (cci == null || cci1 == null || cci2 == null || cci3 == null || cci4 == null)
            {
                return false;
            }

            return cci.Value <= Constants.CciOversold ||
               cci1.Value <= Constants.CciOversold ||
                cci2.Value <= Constants.CciOversold ||
                cci3.Value <= Constants.CciOversold ||
                cci4.Value <= Constants.CciOversold;
        }

        private static bool GetPivotSupportOversoldCondition(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var pivot = candlesticks[currentIndex].StandardPivotPoints.FirstOrDefault();
            var pivot1 = currentIndex >= 1 ? candlesticks[currentIndex - 1].StandardPivotPoints.FirstOrDefault() : null;
            var pivot2 = currentIndex >= 2 ? candlesticks[currentIndex - 2].StandardPivotPoints.FirstOrDefault() : null;
            var pivot3 = currentIndex >= 3 ? candlesticks[currentIndex - 3].StandardPivotPoints.FirstOrDefault() : null;
            var pivot4 = currentIndex >= 4 ? candlesticks[currentIndex - 4].StandardPivotPoints.FirstOrDefault() : null;

            if (pivot == null || pivot1 == null || pivot2 == null || pivot3 == null || pivot4 == null)
            {
                return false;
            }

            return
                candlesticks[currentIndex].ClosePrice <= pivot.PivotPoint ||
                candlesticks[currentIndex - 1].ClosePrice <= pivot1.PivotPoint ||
                candlesticks[currentIndex - 2].ClosePrice <= pivot2.PivotPoint ||
                candlesticks[currentIndex - 3].ClosePrice <= pivot3.PivotPoint ||
                candlesticks[currentIndex - 4].ClosePrice <= pivot4.PivotPoint;
        }

        private static bool GetOnBalanceVolumeCondition(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var obvLowest = candlesticks[currentIndex].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value;
            var obvLowest1 = currentIndex >= 1 ? candlesticks[currentIndex - 1].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value : null;
            var obvLowest2 = currentIndex >= 2 ? candlesticks[currentIndex - 2].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value : null;
            var obvLowest3 = currentIndex >= 3 ? candlesticks[currentIndex - 3].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value : null;
            var obvLowest4 = currentIndex >= 4 ? candlesticks[currentIndex - 4].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value : null;

            if (obvLowest == null || obvLowest1 == null || obvLowest2 == null || obvLowest3 == null || obvLowest4 == null)
            {
                return false;
            }

            var obv = candlesticks[currentIndex].OnBalanceVolumes.FirstOrDefault()?.Value;
            var obv1 = currentIndex >= 1 ? candlesticks[currentIndex - 1].OnBalanceVolumes.FirstOrDefault()?.Value : null;
            var obv2 = currentIndex >= 2 ? candlesticks[currentIndex - 2].OnBalanceVolumes.FirstOrDefault()?.Value : null;
            var obv3 = currentIndex >= 3 ? candlesticks[currentIndex - 3].OnBalanceVolumes.FirstOrDefault()?.Value : null;
            var obv4 = currentIndex >= 4 ? candlesticks[currentIndex - 4].OnBalanceVolumes.FirstOrDefault()?.Value : null;

            if (obv == null || obv1 == null || obv2 == null || obv3 == null || obv4 == null)
            {
                return false;
            }

            return obvLowest <= (decimal?)obv &&
                obvLowest1 <= (decimal?)obv1 &&
                obvLowest2 <= (decimal?)obv2 &&
                obvLowest3 <= (decimal?)obv3 &&
                obvLowest4 <= (decimal?)obv4;
        }

        private static bool GetPsychologicalLineCondition(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var valueLowest = candlesticks[currentIndex].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value;
            var valueLowest1 = currentIndex >= 1 ? candlesticks[currentIndex - 1].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value : null;
            var valueLowest2 = currentIndex >= 2 ? candlesticks[currentIndex - 2].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value : null;
            var valueLowest3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value : null;
            var valueLowest4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Lowests.Find(l => l.PriceType is PriceType.OnBalanceVolume)?.Value : null;

            if (valueLowest == null || valueLowest1 == null || valueLowest2 == null || valueLowest3 == null || valueLowest4 == null)
            {
                return false;
            }

            var value = candlesticks[currentIndex].OnBalanceVolumes.FirstOrDefault()?.Value;
            var value1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].OnBalanceVolumes.FirstOrDefault()?.Value : null;
            var value2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].OnBalanceVolumes.FirstOrDefault()?.Value : null;
            var value3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].OnBalanceVolumes.FirstOrDefault()?.Value : null;
            var value4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].OnBalanceVolumes.FirstOrDefault()?.Value : null;

            if (value == null || value1 == null || value2 == null || value3 == null || value4 == null)
            {
                return false;
            }

            return valueLowest <= (decimal?)value &&
                valueLowest1 <= (decimal?)value1 &&
                valueLowest2 <= (decimal?)value2 &&
                valueLowest3 <= (decimal?)value3 &&
                valueLowest4 <= (decimal?)value4;
        }

        private static bool GetVolumeCondition(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var volume = candlesticks[currentIndex].Volume;
            var volume1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Volume : null;
            var volume2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Volume : null;
            var volume3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Volume : null;
            var volume4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Volume : null;

            if (volume == null || volume1 == null || volume2 == null || volume3 == null || volume4 == null)
            {
                return false;
            }

            decimal minVolume = Math.Min((decimal)volume1, Math.Min((decimal)volume2, Math.Min((decimal)volume3, (decimal)volume4)));
            decimal maxVolume = Math.Max((decimal)volume1, Math.Max((decimal)volume2, Math.Max((decimal)volume3, (decimal)volume4)));

            return volume <= minVolume || volume >= maxVolume;
        }

        private static bool GetOversoldMacdConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var macd = candlesticks[currentIndex].Macds.FirstOrDefault();
            var macd1 = currentIndex - 1 >= 0 && currentIndex - 1 < candlesticks.Count ? candlesticks[currentIndex - 1].Macds.FirstOrDefault() : null;
            var macd2 = currentIndex - 2 >= 0 && currentIndex - 2 < candlesticks.Count ? candlesticks[currentIndex - 2].Macds.FirstOrDefault() : null;
            var macd3 = currentIndex - 3 >= 0 && currentIndex - 3 < candlesticks.Count ? candlesticks[currentIndex - 3].Macds.FirstOrDefault() : null;
            var macd4 = currentIndex - 4 >= 0 && currentIndex - 4 < candlesticks.Count ? candlesticks[currentIndex - 4].Macds.FirstOrDefault() : null;

            return macd?.Histogram <= Constants.MacdOversold ||
                macd1?.Histogram <= Constants.MacdOversold ||
                macd2?.Histogram <= Constants.MacdOversold ||
                macd3?.Histogram <= Constants.MacdOversold ||
                macd4?.Histogram <= Constants.MacdOversold;
        }

        private static bool GetOversoldRateOfChange(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var roc = candlesticks[currentIndex].RateOfChanges.FirstOrDefault();
            var roc1 = currentIndex - 1 >= 0 && currentIndex - 1 < candlesticks.Count ? candlesticks[currentIndex - 1].RateOfChanges.FirstOrDefault() : null;
            var roc2 = currentIndex - 2 >= 0 && currentIndex - 2 < candlesticks.Count ? candlesticks[currentIndex - 2].RateOfChanges.FirstOrDefault() : null;
            var roc3 = currentIndex - 3 >= 0 && currentIndex - 3 < candlesticks.Count ? candlesticks[currentIndex - 3].RateOfChanges.FirstOrDefault() : null;
            var roc4 = currentIndex - 4 >= 0 && currentIndex - 4 < candlesticks.Count ? candlesticks[currentIndex - 4].RateOfChanges.FirstOrDefault() : null;

            return roc?.Value <= Constants.RateOfChangeOversold ||
                roc1?.Value <= Constants.RateOfChangeOversold ||
                roc2?.Value <= Constants.RateOfChangeOversold ||
                roc3?.Value <= Constants.RateOfChangeOversold ||
                roc4?.Value <= Constants.RateOfChangeOversold;
        }

        private static bool GetRateOfChangeEquilibrium(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var rateOfChange = candlesticks[currentIndex].RateOfChanges.FirstOrDefault();
            var rateOfChange1 = currentIndex - 1 >= 0 && currentIndex - 1 < candlesticks.Count ? candlesticks[currentIndex - 1].RateOfChanges.FirstOrDefault() : null;

            return rateOfChange?.Value >= rateOfChange1?.Value;
        }

        private static bool GetOverboughtRsiConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var rsi = candlesticks[currentIndex].Rsis.FirstOrDefault();
            var rsi1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Rsis?.FirstOrDefault() : null;
            var rsi2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Rsis?.FirstOrDefault() : null;
            var rsi3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Rsis?.FirstOrDefault() : null;
            var rsi4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Rsis?.FirstOrDefault() : null;

            return rsi?.Value >= Constants.RsiOverbought ||
                rsi1?.Value >= Constants.RsiOverbought ||
                rsi2?.Value >= Constants.RsiOverbought ||
                rsi3?.Value >= Constants.RsiOverbought ||
                rsi4?.Value >= Constants.RsiOverbought;
        }

        private static bool GetOverboughtStochasticConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var stochastic = candlesticks[currentIndex].Stochastics.FirstOrDefault();
            var stochastic1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Stochastics?.FirstOrDefault() : null;
            var stochastic2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Stochastics?.FirstOrDefault() : null;
            var stochastic3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Stochastics?.FirstOrDefault() : null;
            var stochastic4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Stochastics?.FirstOrDefault() : null;

            return stochastic?.OscillatorK >= Constants.StochasticOverbought ||
                stochastic1?.OscillatorK >= Constants.StochasticOverbought ||
                stochastic2?.OscillatorK >= Constants.StochasticOverbought ||
                stochastic3?.OscillatorK >= Constants.StochasticOverbought ||
                stochastic4?.OscillatorK >= Constants.StochasticOverbought ||
                stochastic?.SignalD >= Constants.StochasticOverbought ||
                stochastic1?.SignalD >= Constants.StochasticOverbought ||
                stochastic2?.SignalD >= Constants.StochasticOverbought ||
                stochastic3?.SignalD >= Constants.StochasticOverbought ||
                stochastic4?.SignalD >= Constants.StochasticOverbought;
        }

        private static bool GetOverboughtAdxConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var adx = candlesticks[currentIndex].Adxs.FirstOrDefault();
            var adx1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Adxs?.FirstOrDefault() : null;
            var adx2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Adxs?.FirstOrDefault() : null;
            var adx3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Adxs?.FirstOrDefault() : null;
            var adx4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Adxs?.FirstOrDefault() : null;

            return adx?.PlusDi >= Constants.AdxOverbought ||
                adx1?.PlusDi >= Constants.AdxOverbought ||
                adx2?.PlusDi >= Constants.AdxOverbought ||
                adx3?.PlusDi >= Constants.AdxOverbought ||
                adx4?.PlusDi >= Constants.AdxOverbought;
        }

        private static bool GetOverboughtBollingerConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var bollingerBand = candlesticks[currentIndex].BollingerBands.FirstOrDefault();
            var bollingerBand1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].BollingerBands.FirstOrDefault() : null;
            var bollingerBand2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].BollingerBands.FirstOrDefault() : null;
            var bollingerBand3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].BollingerBands.FirstOrDefault() : null;
            var bollingerBand4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].BollingerBands.FirstOrDefault() : null;

            if (bollingerBand == null
                || bollingerBand1 == null
                || bollingerBand2 == null
                || bollingerBand3 == null
                || bollingerBand4 == null)
            {
                return false;
            }

            if (!decimal.TryParse(bollingerBand.UpperBand.ToString(), out decimal _)
                || !decimal.TryParse(bollingerBand1.UpperBand.ToString(), out decimal _)
                || !decimal.TryParse(bollingerBand2.UpperBand.ToString(), out decimal _)
                || !decimal.TryParse(bollingerBand3.UpperBand.ToString(), out decimal _)
                || !decimal.TryParse(bollingerBand4.UpperBand.ToString(), out decimal _))
            {
                return false;
            }

            return candlesticks[currentIndex].HighPrice >= bollingerBand.UpperBand
                || candlesticks[currentIndex - 1].HighPrice >= bollingerBand1.UpperBand
                || candlesticks[currentIndex - 2].HighPrice >= bollingerBand2.UpperBand
                || candlesticks[currentIndex - 3].HighPrice >= bollingerBand3.UpperBand
                || candlesticks[currentIndex - 4].HighPrice >= bollingerBand4.UpperBand;
        }

        private static bool GetOverboughtKeltnerConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var keltnerChannel = candlesticks[currentIndex].KeltnerChannels.FirstOrDefault();
            var keltnerChannel1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].KeltnerChannels.FirstOrDefault() : null;
            var keltnerChannel2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].KeltnerChannels.FirstOrDefault() : null;
            var keltnerChannel3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].KeltnerChannels.FirstOrDefault() : null;
            var keltnerChannel4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].KeltnerChannels.FirstOrDefault() : null;

            if (keltnerChannel == null || keltnerChannel1 == null || keltnerChannel2 == null || keltnerChannel3 == null || keltnerChannel4 == null)
            {
                return false;
            }

            if (!decimal.TryParse(keltnerChannel.UpperBand.ToString(), out decimal _)
             || !decimal.TryParse(keltnerChannel1.UpperBand.ToString(), out decimal _)
             || !decimal.TryParse(keltnerChannel2.UpperBand.ToString(), out decimal _)
             || !decimal.TryParse(keltnerChannel3.UpperBand.ToString(), out decimal _)
             || !decimal.TryParse(keltnerChannel4.UpperBand.ToString(), out decimal _))
            {
                return false;
            }

            return candlesticks[currentIndex].HighPrice >= (decimal)keltnerChannel.UpperBand
                || candlesticks[currentIndex - 1].HighPrice >= (decimal)keltnerChannel1.UpperBand
                || candlesticks[currentIndex - 2].HighPrice >= (decimal)keltnerChannel2.UpperBand
                || candlesticks[currentIndex - 3].HighPrice >= (decimal)keltnerChannel3.UpperBand
                || candlesticks[currentIndex - 4].HighPrice >= (decimal)keltnerChannel4.UpperBand;
        }

        private static bool GetOverboughtDonchianConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var donchianChannel = candlesticks[currentIndex].DonchianChannels.FirstOrDefault();
            var donchianChannel1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].DonchianChannels.FirstOrDefault() : null;
            var donchianChannel2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].DonchianChannels.FirstOrDefault() : null;
            var donchianChannel3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].DonchianChannels.FirstOrDefault() : null;
            var donchianChannel4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].DonchianChannels.FirstOrDefault() : null;

            if (donchianChannel == null || donchianChannel1 == null || donchianChannel2 == null || donchianChannel3 == null || donchianChannel4 == null)
            {
                return false;
            }

            return (candlesticks[currentIndex]?.HighPrice != null && donchianChannel.UpperBand is not null && candlesticks[currentIndex].HighPrice >= (decimal)donchianChannel.UpperBand)
                || (candlesticks[currentIndex - 1]?.HighPrice is not null && donchianChannel1.UpperBand != null && candlesticks[currentIndex - 1].HighPrice >= (decimal)donchianChannel1.UpperBand)
                || (candlesticks[currentIndex - 2]?.HighPrice is not null && donchianChannel2.UpperBand != null && candlesticks[currentIndex - 2].HighPrice >= (decimal)donchianChannel2.UpperBand)
                || (candlesticks[currentIndex - 3]?.HighPrice != null && donchianChannel3.UpperBand is not null && candlesticks[currentIndex - 3].HighPrice >= (decimal)donchianChannel3.UpperBand)
                || (candlesticks[currentIndex - 4]?.HighPrice != null && donchianChannel4.UpperBand is not null && candlesticks[currentIndex - 4].HighPrice >= (decimal)donchianChannel4.UpperBand);
        }

        private static bool GetOverboughtAroonConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var aroon = candlesticks[currentIndex].Aroons.FirstOrDefault();
            var aroon1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Aroons.FirstOrDefault() : null;
            var aroon2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Aroons.FirstOrDefault() : null;
            var aroon3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Aroons.FirstOrDefault() : null;
            var aroon4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Aroons.FirstOrDefault() : null;

            return aroon?.AroonUp >= 80 || aroon?.AroonDown <= 20
                || aroon1?.AroonUp >= 80 || aroon1?.AroonDown <= 20
                || aroon2?.AroonUp >= 80 || aroon2?.AroonDown <= 20
                || aroon3?.AroonUp >= 80 || aroon3?.AroonDown <= 20
                || aroon4?.AroonUp >= 80 || aroon4?.AroonDown <= 20;
        }

        private static bool GetOverboughtCciConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var cci = candlesticks[currentIndex].Ccis.FirstOrDefault();
            var cci1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].Ccis.FirstOrDefault() : null;
            var cci2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].Ccis.FirstOrDefault() : null;
            var cci3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].Ccis.FirstOrDefault() : null;
            var cci4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].Ccis.FirstOrDefault() : null;

            if (cci == null || cci1 == null || cci2 == null || cci3 == null || cci4 == null)
            {
                return false;
            }

            return cci.Value >= Constants.CciOverBought ||
               cci1.Value >= Constants.CciOverBought ||
                cci2.Value >= Constants.CciOverBought ||
                cci3.Value >= Constants.CciOverBought ||
                cci4.Value >= Constants.CciOverBought;
        }

        private static bool GetPivotSupportOverboughtConditions(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var pivot = candlesticks[currentIndex].StandardPivotPoints.FirstOrDefault();
            var pivot1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].StandardPivotPoints.FirstOrDefault() : null;
            var pivot2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].StandardPivotPoints.FirstOrDefault() : null;
            var pivot3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].StandardPivotPoints.FirstOrDefault() : null;
            var pivot4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].StandardPivotPoints.FirstOrDefault() : null;

            if (pivot == null || pivot1 == null || pivot2 == null || pivot3 == null || pivot4 == null)
            {
                return false;
            }

            return
                candlesticks[currentIndex].ClosePrice >= pivot.PivotPoint ||
                candlesticks[currentIndex - 1].ClosePrice >= pivot1.PivotPoint ||
                candlesticks[currentIndex - 2].ClosePrice >= pivot2.PivotPoint ||
                candlesticks[currentIndex - 3].ClosePrice >= pivot3.PivotPoint ||
                candlesticks[currentIndex - 4].ClosePrice >= pivot4.PivotPoint;
        }

        private static bool GetHighestHighCandlestickConditions(List<CandlestickExtended> candlesticks, int currentIndex, decimal? candlestickHighestPrice)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var highprice = candlesticks[currentIndex].HighPrice;
            var highprice1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1].HighPrice : null;
            var highprice2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2].HighPrice : null;
            var highprice3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3].HighPrice : null;
            var highprice4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4].HighPrice : null;

            if (highprice == null || highprice1 == null || highprice2 == null || highprice3 == null || highprice4 == null)
            {
                return false;
            }

            return candlestickHighestPrice == highprice
                || candlestickHighestPrice == highprice1
                || candlestickHighestPrice == highprice2
                || candlestickHighestPrice == highprice3
                || candlestickHighestPrice == highprice4;
        }

        private static bool GetFractalBearCondition(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var candlestick2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2] : null;
            var candlestick3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3] : null;
            var candlestick4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4] : null;

            if (candlestick2 is null ||
                candlestick3 is null ||
                candlestick4 is null)
            {
                return false;
            }

            return candlestick2.Fractals.Find(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2)?.Value.HasValue == true ||
                   candlestick3.Fractals.Find(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2)?.Value.HasValue == true ||
                   candlestick4.Fractals.Find(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2)?.Value.HasValue == true;
        }

        private static bool GetFractalEnhancedShortTrend(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var candlestick = candlesticks[currentIndex];
            var candlestick1 = currentIndex - 1 >= 0 ? candlesticks[currentIndex - 1] : null;
            var candlestick2 = currentIndex - 2 >= 0 ? candlesticks[currentIndex - 2] : null;
            var candlestick3 = currentIndex - 3 >= 0 ? candlesticks[currentIndex - 3] : null;
            var candlestick4 = currentIndex - 4 >= 0 ? candlesticks[currentIndex - 4] : null;

            if (candlestick is null ||
                candlestick1 is null ||
                candlestick2 is null ||
                candlestick3 is null ||
                candlestick4 is null)
            {
                return false;
            }

            return candlestick.FractalTrend is Trend.Up ||
                candlestick1.FractalTrend is Trend.Up ||
                candlestick2.FractalTrend is Trend.Up ||
                candlestick3.FractalTrend is Trend.Up ||
                candlestick4.FractalTrend is Trend.Up ||
                candlestick.FractalTrend is Trend.Sideways ||
                candlestick1.FractalTrend is Trend.Sideways ||
                candlestick2.FractalTrend is Trend.Sideways ||
                candlestick3.FractalTrend is Trend.Sideways ||
                candlestick4.FractalTrend is Trend.Sideways;
        }

        private static bool GetPriceEnhancedShortTrend(List<CandlestickExtended> candlesticks, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= candlesticks.Count)
            {
                return false;
            }

            var candlestick = candlesticks[currentIndex];
            var candlestick1 = currentIndex >= 1 ? candlesticks[currentIndex - 1] : null;
            var candlestick2 = currentIndex >= 2 ? candlesticks[currentIndex - 2] : null;
            var candlestick3 = currentIndex >= 3 ? candlesticks[currentIndex - 3] : null;
            var candlestick4 = currentIndex >= 4 ? candlesticks[currentIndex - 4] : null;

            if (candlestick is null ||
                candlestick1 is null ||
                candlestick2 is null ||
                candlestick3 is null ||
                candlestick4 is null)
            {
                return false;
            }

            return candlestick.PriceTrend is Trend.Up ||
                candlestick1.PriceTrend is Trend.Up ||
                candlestick2.PriceTrend is Trend.Up ||
                candlestick3.PriceTrend is Trend.Up ||
                candlestick4.PriceTrend is Trend.Up;
        }
    }
}
