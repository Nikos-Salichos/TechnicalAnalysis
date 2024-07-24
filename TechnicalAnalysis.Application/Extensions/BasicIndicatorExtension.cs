using Microsoft.Extensions.Logging;
using OoplesFinance.StockIndicators;
using OoplesFinance.StockIndicators.Enums;
using OoplesFinance.StockIndicators.Models;
using Skender.Stock.Indicators;
using System.Collections.Frozen;
using System.Collections.Immutable;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.Indicators.Basic;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class BasicIndicatorExtension
    {
        public static ILogger Logger { get; set; }

        public static void CalculateBasicIndicators(this PairExtended pair)
        {
            Logger.LogInformation("Pair details - {PairPropertyName}: {PairName}, " +
                "{PairPropertyContractAddress}: {PairContractAddress}, " +
                "{BaseAssetContractPropertyName}: {BaseAssetContract}, " +
                "{BaseAssetNamePropertyName}: {BaseAssetName}, " +
                "{QuoteAssetContractPropertyName}: {QuoteAssetContract}, " +
                "{QuoteAssetNamePropertyName}: {QuoteAssetName}",
                nameof(pair.Symbol), pair.Symbol,
                nameof(pair.ContractAddress), pair.ContractAddress,
                nameof(pair.BaseAssetContract), pair.BaseAssetContract,
                nameof(pair.BaseAssetName), pair.BaseAssetName,
                nameof(pair.QuoteAssetContract), pair.QuoteAssetContract,
                nameof(pair.QuoteAssetName), pair.QuoteAssetName);

            pair.Candlesticks = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();

            var quotes = pair
                .Candlesticks
                .Where(candlestick =>
                candlestick.OpenPrice.HasValue
                && candlestick.HighPrice.HasValue
                && candlestick.ClosePrice.HasValue
                && candlestick.LowPrice.HasValue
                && candlestick.Volume.HasValue)
                .Select(candlestick => new Quote
                {
                    Open = candlestick.OpenPrice.Value,
                    High = candlestick.HighPrice.Value,
                    Low = candlestick.LowPrice.Value,
                    Close = candlestick.ClosePrice.Value,
                    Date = candlestick.CloseDate,
                    Volume = candlestick.Volume.Value
                })
                .OrderBy(q => q.Date).ToFrozenSet();

            var candlestickLookup = pair.Candlesticks.ToImmutableDictionary(c => c.CloseDate);

            CalculateStochastic(quotes, candlestickLookup);
            CalculateRsi(quotes, candlestickLookup);
            CalculateAdx(quotes, candlestickLookup);
            CalculateBollingerBands(quotes, candlestickLookup);
            CalculateDonchianChannel(quotes, candlestickLookup);
            CalculateKeltnerChannel(quotes, candlestickLookup);
            CalculateCci(quotes, candlestickLookup);
            CalculateAroon(quotes, candlestickLookup);
            CalculateSma(quotes, candlestickLookup);
            CalculateIchimoku(quotes, candlestickLookup);
            CalculateMacd(quotes, candlestickLookup);
            CalculateFractals(quotes, candlestickLookup);
            CalculateStandardDeviation(quotes, candlestickLookup);
            CalculateRateOfChange(quotes, candlestickLookup);
            CalculateStandardPivotPoints(quotes, candlestickLookup);
            CalculateAverageTrueRange(quotes, candlestickLookup);

            CalculateFractalLowestLow(pair);
            CalculateFractalLowestHigh(pair);
            CalculateHighestHigh(pair);
            CalculateLowestLow(pair);
            CalculateHighestClose(pair);
            CalculateStatisticsFromAllTimeHighLow(pair);

            CalculateHistoricalVolatility(pair);
            CalculateAverageRange(pair);
            CalculateVerticalHorizontalFilter(pair);
            CalculateOnBalanceVolumeOscilator(pair, quotes, candlestickLookup);
            CalculatePsychologicalLine(pair);

            CalculateRsiExtreme(pair.Candlesticks);
        }

        public static void CalculateRsiExtreme(List<CandlestickExtended> candlesticks)
        {
            const int lookbackPeriod = 30;

            for (int i = 0; i < candlesticks.Count; i++)
            {
                // Determine the starting index for the lookback period
                int start = Math.Max(0, i - lookbackPeriod + 1);

                // Get the chunk of candlesticks for the current lookback period
                var chunk = candlesticks.Skip(start).Take(lookbackPeriod);

                var rsiValues = chunk
                     .Where(c => c.Rsis?.Count > 0 && c.Rsis[0]?.Value != null)
                     .Select(c => c.Rsis[0].Value)
                     .ToList();

                if (rsiValues.Count is 0)
                {
                    continue;
                }

                // Find the highest and lowest RSI values within the lookback period
                var highestRsi = rsiValues.Max();
                var lowestRsi = rsiValues.Min();

                // Count the number of RSI values that are lower than the current RSI value
                if (candlesticks[i].Rsis?.Count > 0)
                {
                    int numberOfRsiLowerThanCurrent = rsiValues.Count(rsi => rsi < candlesticks[i].Rsis[0].Value);
                    // Add a new DynamicRsi object to the current candlestick's DynamicRsis collection
                    candlesticks[i].DynamicRsis.Add(new DynamicRsi(candlesticks[i].PrimaryId)
                    {
                        Period = Constants.RsiPeriod,
                        Overbought = highestRsi.Value,
                        Oversold = lowestRsi.Value,
                        Value = candlesticks[i].Rsis[0].Value,
                        NumberOfRsiLowerThanPreviousRsis = numberOfRsiLowerThanCurrent
                    });
                }
            }
        }

        private static void CalculateBollingerBands(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var bollingerBandResult in quotes.Use(CandlePart.Close).GetBollingerBands())
            {
                if (decimal.TryParse(bollingerBandResult.LowerBand.ToString(), out decimal lowerBandValue)
                    && candlestickLookup.TryGetValue(bollingerBandResult.Date, out var candlestick))
                {
                    if (candlestick?.OpenPrice < lowerBandValue)
                    {
                        candlestick.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult.Sma,
                            UpperBand = (decimal?)bollingerBandResult.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult.PercentB,
                            Width = (decimal?)bollingerBandResult.Width,
                            OpenPriceOutOfBollinger = true
                        });
                    }
                    else if (candlestick?.HighPrice < lowerBandValue)
                    {
                        candlestick.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult.Sma,
                            UpperBand = (decimal?)bollingerBandResult.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult.PercentB,
                            Width = (decimal?)bollingerBandResult.Width,
                            WholeCandlestickOutOfBollinger = true
                        });
                    }
                    else if (candlestick?.ClosePrice < lowerBandValue)
                    {
                        candlestick.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult.Sma,
                            UpperBand = (decimal?)bollingerBandResult.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult.PercentB,
                            Width = (decimal?)bollingerBandResult.Width,
                            ClosePriceOutOfBollinger = true
                        });
                    }
                    else if (candlestick?.LowPrice < lowerBandValue)
                    {
                        candlestick.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult.Sma,
                            UpperBand = (decimal?)bollingerBandResult.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult.PercentB,
                            Width = (decimal?)bollingerBandResult.Width,
                            LowPriceOutOfBollinger = true
                        });
                    }
                    else
                    {
                        candlestick?.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult.Sma,
                            UpperBand = (decimal?)bollingerBandResult.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult.PercentB,
                            Width = (decimal?)bollingerBandResult.Width,
                        });
                    }
                }
            }
        }

        private static void CalculateDonchianChannel(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var donchianChannelResult in quotes.GetDonchian())
            {
                if (candlestickLookup.TryGetValue(donchianChannelResult.Date, out var candlestick))
                {
                    var donchian = new DonchianChannel(candlestick.PrimaryId)
                    {
                        Centerline = donchianChannelResult.Centerline,
                        UpperBand = donchianChannelResult.UpperBand,
                        LowerBand = donchianChannelResult.LowerBand,
                        Period = 20,
                        Width = donchianChannelResult.Width
                    };
                    candlestick.DonchianChannels.Add(donchian);
                }
            }
        }

        private static void CalculateKeltnerChannel(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var keltnerChannelResult in quotes.GetKeltner())
            {
                if (candlestickLookup.TryGetValue(keltnerChannelResult.Date, out var candlestick))
                {
                    var keltnerChannel = new KeltnerChannel(candlestick.PrimaryId)
                    {
                        Centerline = keltnerChannelResult.Centerline,
                        LowerBand = keltnerChannelResult.LowerBand,
                        UpperBand = keltnerChannelResult.UpperBand,
                        Period = 20
                    };
                    candlestick.KeltnerChannels.Add(keltnerChannel);
                }
            }
        }

        private static void CalculateStochastic(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            if (quotes.Count <= 13)
            {
                return;
            }

            foreach (var indicatorResult in quotes.GetStoch(14, 3, 1))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var stochastic = new Stochastic(candlestick.PrimaryId)
                    {
                        OscillatorK = indicatorResult.Oscillator,
                        PercentJ = indicatorResult.PercentJ,
                        SignalD = indicatorResult.Signal
                    };
                    candlestick.Stochastics.Add(stochastic);
                }
            }
        }

        private static void CalculateRsi(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            var rsiResults = quotes.GetRsi(Constants.RsiPeriod).ToList();

            for (int i = 0; i < rsiResults.Count; i++)
            {
                var rsiResult = rsiResults[i];

                if (!candlestickLookup.TryGetValue(rsiResult.Date, out var candlestick))
                {
                    continue;
                }

                int numberOfRsiLower = 0;

                for (int j = i - 1; j >= 0; j--)
                {
                    var pastRsiResult = rsiResults[j];

                    if (rsiResult.Rsi < pastRsiResult.Rsi)
                    {
                        numberOfRsiLower++;
                    }
                    else
                    {
                        candlestick.Rsis.Add(new Rsi(candlestick.PrimaryId)
                        {
                            Period = Constants.RsiPeriod,
                            Overbought = Constants.RsiOverbought,
                            Oversold = Constants.RsiOversold,
                            Value = rsiResult.Rsi,
                            NumberOfRsiLowerThanPreviousRsis = numberOfRsiLower
                        });
                        break;
                    }
                }
            }

            double? previousRsiValue = null;
            long consecutiveLowerRsiCount = 0;
            long consecutiveHigherRsiCount = 0;

            foreach (var rsi in candlestickLookup.Values.SelectMany(candlestick => candlestick.Rsis.Where(r => r.Period == Constants.RsiPeriod)))
            {
                if (previousRsiValue.HasValue)
                {
                    if (previousRsiValue.Value < 50 && rsi.Value >= 50)
                    {
                        rsi.RsiChangedDirectionFromPreviousCandlestick = true;
                    }
                    if (previousRsiValue.Value >= 50 && rsi.Value < 50)
                    {
                        rsi.RsiChangedDirectionFromPreviousCandlestick = true;
                    }

                    if (previousRsiValue.Value > rsi.Value)
                    {
                        consecutiveLowerRsiCount++;
                        consecutiveHigherRsiCount = 0;
                    }
                    else if (previousRsiValue.Value < rsi.Value)
                    {
                        consecutiveHigherRsiCount++;
                        consecutiveLowerRsiCount = 0;
                    }

                    rsi.NumberOfConsecutiveLowerRsi = consecutiveLowerRsiCount;
                    rsi.NumberOfConsecutiveHigherRsi = consecutiveHigherRsiCount;
                }
                previousRsiValue = rsi.Value;
            }
        }

        private static void CalculateSma(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            int? counter = null;

            foreach (var indicatorResult in quotes.GetSma(Constants.SimpleMovingAveragePeriod))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    candlestick.MovingAverages.Add(new MovingAverage(candlestick.PrimaryId)
                    {
                        MovingAverageType = MovingAverageType.Simple,
                        Period = Constants.SimpleMovingAveragePeriod,
                        Value = indicatorResult.Sma
                    });

                    // Compare ClosePrice with SMA and update the counter
                    if (candlestick.ClosePrice < (decimal?)indicatorResult.Sma)
                    {
                        counter++;
                        candlestick.ConsecutiveCandlesticksBelowSma = counter;
                    }
                    else
                    {
                        counter = 0;
                    }
                }
            }
        }

        private static void CalculateAdx(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetAdx())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var adx = new Adx(candlestick.PrimaryId)
                    {
                        PlusDi = indicatorResult.Pdi,
                        MinusDi = indicatorResult.Mdi,
                        Adxr = indicatorResult.Adxr,
                        Value = indicatorResult.Adx
                    };
                    candlestick.Adxs.Add(adx);
                }
            }
        }

        private static void CalculateAroon(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetAroon())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    candlestick.Aroons.Add(new Aroon(candlestick.PrimaryId)
                    {
                        AroonUp = indicatorResult.AroonUp,
                        AroonDown = indicatorResult.AroonDown,
                        Oscillator = indicatorResult.Oscillator,
                        Period = 25,
                    });
                }
            }
        }

        private static void CalculateCci(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetCci())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    candlestick.Ccis.Add(new Cci(candlestick.PrimaryId)
                    {
                        Period = 20,
                        Value = indicatorResult.Cci,
                        Overbought = 100,
                        Oversold = 100
                    });
                }
            }
        }

        private static void CalculateFractals(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetFractal(2))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    if (indicatorResult.FractalBull is not null)
                    {
                        var bullFractal = new Fractal(candlestick.PrimaryId)
                        {
                            FractalType = FractalType.BullFractal,
                            WindowPeriod = 2,
                            Value = indicatorResult.FractalBull
                        };
                        candlestick.Fractals.Add(bullFractal);
                    }

                    if (indicatorResult.FractalBear is not null)
                    {
                        var bearFractal = new Fractal(candlestick.PrimaryId)
                        {
                            FractalType = FractalType.BearFractal,
                            WindowPeriod = 2,
                            Value = indicatorResult.FractalBear
                        };
                        candlestick.Fractals.Add(bearFractal);
                    }
                }
            }
        }

        private static void CalculateIchimoku(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetIchimoku())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var ichimoku = new Ichimoku(candlestick.PrimaryId)
                    {
                        ChikouSpan = indicatorResult.ChikouSpan,
                        KijunSen = indicatorResult.KijunSen,
                        SenkouSpanA = indicatorResult.SenkouSpanA,
                        SenkouSpanB = indicatorResult.SenkouSpanB,
                        TenkanSen = indicatorResult.TenkanSen
                    };
                    candlestick.Ichimokus.Add(ichimoku);
                }
            }
        }

        private static void CalculateStandardDeviation(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetStdDev(20))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var stdDev = new StandardDeviation(candlestick.PrimaryId)
                    {
                        Mean = indicatorResult.Mean,
                        Period = 20,
                        StadardDeviationValue = indicatorResult.StdDev,
                        StdDevSma = indicatorResult.StdDevSma,
                        ZScore = indicatorResult.ZScore,
                    };
                    candlestick.StandardDeviations.Add(stdDev);
                }
            }
        }

        private static void CalculateRateOfChange(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetRoc(Constants.RateOfChangePeriod))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var roc = new RateOfChange(candlestick.PrimaryId)
                    {
                        Period = Constants.RateOfChangePeriod,
                        Value = indicatorResult.Roc
                    };
                    candlestick.RateOfChanges.Add(roc);
                }
            }
        }

        private static void CalculateStandardPivotPoints(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetPivotPoints(PeriodSize.Day, PivotPointType.Standard))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var pivotPoint = new StandardPivotPoint(candlestick.PrimaryId)
                    {
                        PivotPoint = indicatorResult.PP,
                        Support1 = indicatorResult.S1,
                        Support2 = indicatorResult.S2,
                        Support3 = indicatorResult.S3,
                        Resistance1 = indicatorResult.R1,
                        Resistance2 = indicatorResult.R2,
                        Resistance3 = indicatorResult.R3,
                        Timeframe = Timeframe.Daily
                    };
                    candlestick.StandardPivotPoints.Add(pivotPoint);
                }
            }
        }

        private static void CalculateMacd(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetMacd(fastPeriods: 7, slowPeriods: 14, signalPeriods: 9))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var macd = new Macd(candlestick.PrimaryId)
                    {
                        FastEma = indicatorResult.FastEma,
                        SlowEma = indicatorResult.SlowEma,
                        Histogram = indicatorResult.Histogram,
                        MacdValue = indicatorResult.Macd,
                        Signal = indicatorResult.Signal,
                    };
                    candlestick.Macds.Add(macd);
                }
            }
        }

        private static void CalculateAverageTrueRange(FrozenSet<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetAtr())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    candlestick?.AverageTrueRanges.Add(new AverageTrueRange(candlestick.PrimaryId)
                    {
                        AverageTrueRangePercent = (decimal?)indicatorResult.Atrp,
                        AverageTrueRangeValue = (decimal?)indicatorResult.Atr,
                        TrueRange = (decimal?)indicatorResult.Tr,
                        Period = 14
                    });
                }
            }
        }

        private static void CalculateLowestLow(PairExtended pair)
        {
            const int period = 5;

            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                int startIndex = Math.Max(i - (period - 1), 0);
                var group = pair.Candlesticks.GetRange(startIndex, i - startIndex + 1);
                var lowestLow = group.Min(c => c.LowPrice);

                var candlestick = pair.Candlesticks[i];
                candlestick.Lowests.Add(new Lowest(candlestick.PrimaryId)
                {
                    Period = period,
                    Value = lowestLow,
                    PriceType = PriceType.Low
                });
            }
        }

        private static void CalculateStatisticsFromAllTimeHighLow(PairExtended pair)
        {
            if (pair.Candlesticks.Count == 0)
            {
                return;
            }

            decimal? maxHighPriceSoFar = 0;
            decimal? minimumLowPriceSoFar = 0;
            int lastAllTimeHighIndex = -1;

            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                var currentCandlestick = pair.Candlesticks[i];

                if (currentCandlestick.HighPrice > maxHighPriceSoFar)
                {
                    maxHighPriceSoFar = currentCandlestick.HighPrice;
                    lastAllTimeHighIndex = i;
                }

                if (currentCandlestick.LowPrice < minimumLowPriceSoFar)
                {
                    minimumLowPriceSoFar = currentCandlestick.LowPrice;
                }

                currentCandlestick.PercentageFromAllTimeHigh = ((currentCandlestick.ClosePrice / maxHighPriceSoFar) - 1) * 100;
                currentCandlestick.DaysFromAllTimeHigh = i - lastAllTimeHighIndex;
            }

            pair.AllTimeHighPrice = maxHighPriceSoFar;
            pair.AllTimeLowPrice = minimumLowPriceSoFar;
        }

        private static void CalculateHighestHigh(PairExtended pair)
        {
            const int period = 5;

            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                int startIndex = Math.Max(i - (period - 1), 0);
                var group = pair.Candlesticks.GetRange(startIndex, i - startIndex + 1);
                var highestHigh = group.Max(c => c.HighPrice);

                var candlestick = pair.Candlesticks[i];
                candlestick.Highests.Add(new Highest(candlestick.PrimaryId)
                {
                    Period = period,
                    Value = highestHigh,
                    PriceType = PriceType.High
                });
            }
        }

        private static void CalculateHighestClose(PairExtended pair)
        {
            const int period = 22;

            for (int i = period - 1; i < pair.Candlesticks.Count; i++)
            {
                int startIndex = i - (period - 1);
                var group = pair.Candlesticks.GetRange(startIndex, period);
                var highestClose = group.Max(c => c.ClosePrice);

                var candlestick = pair.Candlesticks[i];
                candlestick.Highests.Add(new Highest(candlestick.PrimaryId)
                {
                    Period = period,
                    Value = highestClose,
                    PriceType = PriceType.Close
                });
            }
        }

        private static void CalculateFractalLowestHigh(PairExtended pair)
        {
            var candlesticksWithBearFractals = pair.Candlesticks.Where(c => c.Fractals.Count > 0 && c.Fractals.Find(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2) is not null)
                                                                .OrderBy(c => c.OpenDate)
                                                                .ToList();

            int count = 1;
            for (int i = 0; i < candlesticksWithBearFractals.Count; i++)
            {
                var candlestick = candlesticksWithBearFractals[i];
                var candlestick1 = i >= 1 ? candlesticksWithBearFractals[i - 1] : null;

                if (candlestick1 is null)
                {
                    continue;
                }

                var fractalBear = candlestick.Fractals?.Find(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2);
                var fractalBear1 = candlestick1.Fractals?.Find(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2);

                if (fractalBear?.Value <= fractalBear1?.Value)
                {
                    count++;
                    var candlestickInPair = pair.Candlesticks.Find(c => c.OpenDate == candlestick.OpenDate);
                    candlestickInPair?.FractalLowests.Add(
                        new FractalLowest(candlestick.PrimaryId)
                        {
                            Consecutive = count,
                            Price = candlestick.HighPrice,
                            PriceType = PriceType.High
                        });
                }
                else
                {
                    count = 1;
                }
            }
        }

        private static void CalculateFractalLowestLow(PairExtended pair)
        {
            var candlesticksWithBullFractals = pair.Candlesticks.Where(c => c.Fractals.Count > 0 && c.Fractals.Find(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2) is not null)
                                                                .OrderBy(c => c.OpenDate).ToList();

            int count = 0;
            for (int i = 0; i < candlesticksWithBullFractals.Count; i++)
            {
                var candlestick = candlesticksWithBullFractals[i];
                var candlestick1 = i >= 1 ? candlesticksWithBullFractals[i - 1] : null;

                if (candlestick1 is null)
                {
                    continue;
                }

                var FractalBull = candlestick.Fractals?.Find(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2);
                var FractalBull1 = candlestick1.Fractals?.Find(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2);

                if (FractalBull?.Value <= FractalBull1?.Value)
                {
                    count++;
                    var candlestickInPair = pair.Candlesticks.Find(c => c.OpenDate == candlestick.OpenDate);
                    candlestickInPair?.FractalLowests.Add(new FractalLowest(candlestick.PrimaryId)
                    {
                        Consecutive = count,
                        Price = candlestick.LowPrice,
                        PriceType = PriceType.Low
                    });
                }
                else
                {
                    count = 0;
                }
            }
        }

        private static void CalculateHistoricalVolatility(PairExtended pair)
        {
            var stockData = new StockData(
                 pair.Candlesticks.Select(x => x.OpenPrice != null ? (double)x.OpenPrice : 0.0),
                 pair.Candlesticks.Select(x => x.HighPrice != null ? (double)x.HighPrice : 0.0),
                 pair.Candlesticks.Select(x => x.LowPrice != null ? (double)x.LowPrice : 0.0),
                 pair.Candlesticks.Select(x => x.ClosePrice != null ? (double)x.ClosePrice : 0.0),
                 pair.Candlesticks.Select(x => x.Volume != null ? (double)x.Volume : 0.0),
                 pair.Candlesticks.Select(x => x.CloseDate));

            const int period = 21;

            var results = stockData.CalculateHistoricalVolatility(MovingAvgType.ExponentialMovingAverage, period);

            var hvpValues = results.OutputValues["Hv"];

            for (int counter = 0; counter < pair.Candlesticks.Count; counter++)
            {
                var candlestick = pair.Candlesticks[counter];
                double valueAtIndex = hvpValues.ElementAtOrDefault(counter);
                candlestick?.Volatilities.Add(new Volatility(candlestick.PrimaryId)
                {
                    Period = period,
                    VolatilityValue = valueAtIndex
                });
            }
        }

        private static void CalculateAverageRange(PairExtended pair)
        {
            const int period = 5;

            if (pair.Candlesticks.Count < period)
            {
                return;
            }

            var candlesticks = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();

            for (int i = 4; i < candlesticks.Count; i++)
            {
                decimal? sumRange = 0;

                for (int j = i - 4; j <= i; j++)
                {
                    CandlestickExtended candlestick = candlesticks[j];
                    decimal? range = candlestick.HighPrice - candlestick.LowPrice;
                    sumRange += range;
                }

                decimal? averageRange = sumRange / period;

                var currentCandlestick = candlesticks[i];
                var candlestickInPair = candlesticks.Find(c => c.OpenDate == currentCandlestick.OpenDate);

                candlestickInPair?.AverageRanges.Add(new AverageRange(currentCandlestick.PrimaryId)
                {
                    Period = 5,
                    Value = averageRange
                });
            }
        }

        private static void CalculateVerticalHorizontalFilter(PairExtended pair)
        {
            var stockData = new StockData(
                 pair.Candlesticks.Select(x => x.OpenPrice != null ? (double)x.OpenPrice : 0.0),
                 pair.Candlesticks.Select(x => x.HighPrice != null ? (double)x.HighPrice : 0.0),
                 pair.Candlesticks.Select(x => x.LowPrice != null ? (double)x.LowPrice : 0.0),
                 pair.Candlesticks.Select(x => x.ClosePrice != null ? (double)x.ClosePrice : 0.0),
                 pair.Candlesticks.Select(x => x.Volume != null ? (double)x.Volume : 0.0),
                 pair.Candlesticks.Select(x => x.CloseDate));

            const int period = 21;

            var results = stockData.CalculateVerticalHorizontalFilter(MovingAvgType.ExponentialMovingAverage, period);

            var vhfValues = results.OutputValues["Vhf"];  // Retrieve the "Vhf" values list

            for (int counter = 0; counter < pair.Candlesticks.Count; counter++)
            {
                var candlestick = pair.Candlesticks[counter];
                double valueAtIndex = vhfValues.ElementAtOrDefault(counter);
                candlestick?.VerticalHorizontalFilters.Add(new VerticalHorizontalFilter(candlestick.PrimaryId)
                {
                    Period = period,
                    Value = valueAtIndex
                });
            }
        }

        private static void CalculateOnBalanceVolumeOscilator(PairExtended pair, FrozenSet<Quote> quotes,
            ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetObv())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    candlestick?.OnBalanceVolumes.Add(
                        new OnBalanceVolume(candlestick.PrimaryId)
                        {
                            Value = indicatorResult.Obv
                        });
                }
            }

            CalculateLowestLowOnBalanceVolume();

            void CalculateLowestLowOnBalanceVolume()
            {
                const int period = 5;

                for (int i = 0; i < pair.Candlesticks.Count; i++)
                {
                    int startIndex = Math.Max(i - (period - 1), 0);

                    var lowest = pair.Candlesticks.Skip(startIndex)
                                                     .Take(i - startIndex + 1)
                                                     .Min(c => c.OnBalanceVolumes.FirstOrDefault()?.Value ?? double.MaxValue);

                    if (lowest != double.MaxValue)
                    {
                        pair.Candlesticks[i].Lowests.Add(new Lowest(pair.Candlesticks[i].PrimaryId)
                        {
                            Period = period,
                            Value = (decimal)lowest,
                            PriceType = PriceType.OnBalanceVolume
                        });
                    }
                }
            }
        }

        private static void CalculatePsychologicalLine(PairExtended pair)
        {
            const int period = 5;
            foreach (var candlestickWithIndex in pair.Candlesticks.Select((candlestick, index) => new { candlestick, index }))
            {
                if (candlestickWithIndex.index >= period - 1)
                {
                    int startIndex = Math.Max(candlestickWithIndex.index - period + 1, 0);

                    int risingPeriodsCount = pair.Candlesticks
                            .Skip(startIndex)
                            .Take(candlestickWithIndex.index - startIndex + 1)
                            .Where((cs, i) => i + startIndex > 0 && cs.ClosePrice > pair.Candlesticks[i + startIndex - 1].ClosePrice)
                            .Count();

                    var psyValue = (decimal)risingPeriodsCount / period * 100;

                    candlestickWithIndex.candlestick.PsychologicalLines.Add
                        (new PsychologicalLine(candlestickWithIndex.candlestick.PrimaryId)
                        {
                            Period = period,
                            Value = (double)psyValue
                        });
                }
            }

            CalculateLowestLowPsychologicalLine();

            void CalculateLowestLowPsychologicalLine()
            {
                const int period = 5;

                for (int i = 0; i < pair.Candlesticks.Count; i++)
                {
                    int startIndex = Math.Max(i - (period - 1), 0);

                    var lowest = pair
                        .Candlesticks
                        .Skip(startIndex)
                        .Take(i - startIndex + 1)
                        .Min(c => c.PsychologicalLines.FirstOrDefault()?.Value ?? double.MaxValue);

                    if (lowest != double.MaxValue)
                    {
                        pair.Candlesticks[i].Lowests.Add(new Lowest(pair.Candlesticks[i].PrimaryId)
                        {
                            Period = period,
                            Value = (decimal)lowest,
                            PriceType = PriceType.PsychologicalLine
                        });
                    }
                }
            }
        }
    }
}