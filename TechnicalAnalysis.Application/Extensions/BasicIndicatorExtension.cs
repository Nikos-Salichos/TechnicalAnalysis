using Microsoft.Extensions.Logging;
using OoplesFinance.StockIndicators;
using OoplesFinance.StockIndicators.Enums;
using OoplesFinance.StockIndicators.Models;
using Skender.Stock.Indicators;
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
            Logger.LogInformation("Method name: {MethodName} - Pair details - {PairPropertyName}: {PairName}, " +
                "{PairPropertyContractAddress}: {PairContractAddress}, " +
                "{BaseAssetContractPropertyName}: {BaseAssetContract}, " +
                "{BaseAssetNamePropertyName}: {BaseAssetName}, " +
                "{QuoteAssetContractPropertyName}: {QuoteAssetContract}, " +
                "{QuoteAssetNamePropertyName}: {QuoteAssetName}", nameof(CalculateBasicIndicators),
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
                && candlestick.LowPrice.HasValue)
                .Select(candlestick => new Quote
                {
                    Open = candlestick.OpenPrice.Value,
                    High = candlestick.HighPrice.Value,
                    Low = candlestick.LowPrice.Value,
                    Close = candlestick.ClosePrice.Value,
                    Date = candlestick.CloseDate,
                })
                .OrderBy(q => q.Date);

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

            CalculateVolatility(pair);
            CalculateAverageRange(pair);
        }

        private static void CalculateBollingerBands(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var bollingerBandResult in quotes.Use(CandlePart.Close).GetBollingerBands())
            {
                if (decimal.TryParse(bollingerBandResult.LowerBand.ToString(), out decimal lowerBandValue)
                    && candlestickLookup.TryGetValue(bollingerBandResult.Date, out var candlestick))
                {
                    if (candlestick?.OpenPrice < lowerBandValue)
                    {
                        candlestick?.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult?.Sma,
                            UpperBand = (decimal?)bollingerBandResult?.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult?.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult?.PercentB,
                            Width = (decimal?)bollingerBandResult?.Width,
                            OpenPriceOutOfBollinger = true
                        });
                    }
                    else if (candlestick?.HighPrice < lowerBandValue)
                    {
                        candlestick?.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult?.Sma,
                            UpperBand = (decimal?)bollingerBandResult?.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult?.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult?.PercentB,
                            Width = (decimal?)bollingerBandResult?.Width,
                            WholeCandlestickOutOfBollinger = true
                        });
                    }
                    else if (candlestick?.ClosePrice < lowerBandValue)
                    {
                        candlestick?.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult?.Sma,
                            UpperBand = (decimal?)bollingerBandResult?.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult?.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult?.PercentB,
                            Width = (decimal?)bollingerBandResult?.Width,
                            ClosePriceOutOfBollinger = true
                        });
                    }
                    else if (candlestick?.LowPrice < lowerBandValue)
                    {
                        candlestick?.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult?.Sma,
                            UpperBand = (decimal?)bollingerBandResult?.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult?.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult?.PercentB,
                            Width = (decimal?)bollingerBandResult?.Width,
                            LowPriceOutOfBollinger = true
                        });
                    }
                    else
                    {
                        candlestick?.BollingerBands.Add(new BollingerBand(candlestick.PrimaryId)
                        {
                            Period = 20,
                            StandardDeviation = 2,
                            MiddleBand = (decimal?)bollingerBandResult?.Sma,
                            UpperBand = (decimal?)bollingerBandResult?.UpperBand,
                            LowerBand = (decimal?)bollingerBandResult?.LowerBand,
                            PercentageBandwidth = (decimal?)bollingerBandResult?.PercentB,
                            Width = (decimal?)bollingerBandResult?.Width,
                        });
                    }
                }
            }
        }

        private static void CalculateDonchianChannel(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var donchianChannelResult in quotes.GetDonchian())
            {
                if (candlestickLookup.TryGetValue(donchianChannelResult.Date, out var candlestick))
                {
                    var donchian = DonchianChannel.Create(
                        candlestickId: candlestick.PrimaryId,
                        period: 20,
                        upperBand: donchianChannelResult.UpperBand,
                        centerline: donchianChannelResult.Centerline,
                        lowerBand: donchianChannelResult.LowerBand,
                        width: donchianChannelResult.Width
                    );
                    candlestick.DonchianChannels.Add(donchian);
                }
            }
        }

        private static void CalculateKeltnerChannel(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var keltnerChannelResult in quotes.GetKeltner())
            {
                if (candlestickLookup.TryGetValue(keltnerChannelResult.Date, out var candlestick))
                {
                    var keltner = KeltnerChannel.Create(
                        candlestickId: candlestick.PrimaryId,
                        period: 20,
                        upperBand: keltnerChannelResult.UpperBand,
                        centerline: keltnerChannelResult.Centerline,
                        lowerBand: keltnerChannelResult.LowerBand,
                        width: keltnerChannelResult.Width
                    );
                    candlestick.KeltnerChannels.Add(keltner);
                }
            }
        }

        private static void CalculateStochastic(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            if (quotes.Count() <= 13)
            {
                return;
            }

            foreach (var indicatorResult in quotes.GetStoch(14, 3, 1))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var stochastic = Stochastic.Create(
                        candlestickId: candlestick.PrimaryId,
                        oscillatorK: indicatorResult.Oscillator,
                        signalD: indicatorResult.Signal,
                        percentJ: indicatorResult.PercentJ
                    );
                    candlestick.Stochastics.Add(stochastic);
                }
            }
        }

        private static void CalculateRsi(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
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

        private static void CalculateSma(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetSma(200))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var sma = MovingAverage.Create(
                        candlestickId: candlestick.PrimaryId,
                        period: 200,
                        value: indicatorResult.Sma,
                        movingAverageType: MovingAverageType.Simple
                    );
                    candlestick.MovingAverages.Add(sma);
                }
            }
        }

        private static void CalculateAdx(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
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

        private static void CalculateAroon(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetAroon())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var aroon = Aroon.Create(
                        candlestickId: candlestick.PrimaryId,
                        period: 25,
                        aroonUp: indicatorResult.AroonUp,
                        aroonDown: indicatorResult.AroonDown,
                        oscillator: indicatorResult.Oscillator
                    );
                    candlestick.Aroons.Add(aroon);
                }
            }
        }

        private static void CalculateCci(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetCci())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var cci = Cci.Create(
                        candlestickId: candlestick.PrimaryId,
                        period: 20,
                        value: indicatorResult.Cci,
                        overbought: 100,
                        oversold: -100
                    );
                    candlestick.Ccis.Add(cci);
                }
            }
        }

        private static void CalculateFractals(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
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

        private static void CalculateIchimoku(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
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

        private static void CalculateStandardDeviation(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
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

        private static void CalculateRateOfChange(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
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

        private static void CalculateStandardPivotPoints(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
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
                        Resistance3 = indicatorResult.R3
                    };
                    candlestick.StandardPivotPoints.Add(pivotPoint);
                }
            }
        }

        private static void CalculateMacd(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetMacd(7, 14, 9))
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    var macd = Macd.Create(
                        candlestickId: candlestick.PrimaryId,
                        macdValue: indicatorResult.Macd,
                        signal: indicatorResult.Signal,
                        histogram: indicatorResult.Histogram,
                        fastEma: indicatorResult.FastEma,
                        slowEma: indicatorResult.SlowEma
                    );
                    candlestick.Macds.Add(macd);
                }
            }
        }

        private static void CalculateAverageTrueRange(IEnumerable<Quote> quotes, ImmutableDictionary<DateTime, CandlestickExtended> candlestickLookup)
        {
            foreach (var indicatorResult in quotes.GetAtr())
            {
                if (candlestickLookup.TryGetValue(indicatorResult.Date, out var candlestick))
                {
                    candlestick?.AverageTrueRanges.Add(new AverageTrueRange(candlestick.PrimaryId,
                        period: 14,
                        (decimal?)indicatorResult.Tr,
                        (decimal?)indicatorResult.Atr,
                        (decimal?)indicatorResult.Atrp));
                }
            }
        }

        private static void CalculateLowestLow(PairExtended pair)
        {
            const int period = 5;
            int count = pair.Candlesticks.Count;

            for (int i = 0; i < count; i++)
            {
                int startIndex = Math.Max(i - (period - 1), 0); // Start from the current candlestick and go back 4 candlesticks or less if there are fewer than 5 candlesticks available

                // Validate indices and range
                if (startIndex >= 0 && startIndex < count && // Make sure startIndex is valid
                    i >= 0 && i < count &&                   // Make sure i is valid
                    i - startIndex + 1 <= count - startIndex) // Make sure the range is valid
                {
                    var group = pair.Candlesticks.GetRange(startIndex, i - startIndex + 1);

                    var lowestLow = group.Min(c => c.LowPrice);

                    foreach (var candlestick in group)
                    {
                        candlestick.Lowests.Add(new Lowest(candlestick.PrimaryId)
                        {
                            Period = period,
                            Value = lowestLow,
                            PriceType = PriceType.Low
                        });
                    }
                }
            }
        }

        private static void CalculateHighestHigh(PairExtended pair)
        {
            const int period = 5;
            int count = pair.Candlesticks.Count;

            for (int i = 0; i < count; i++)
            {
                int startIndex = Math.Max(i - (period - 1), 0); // Start from the current candlestick and go back 4 candlesticks or less if there are fewer than 5 candlesticks available
                var group = pair.Candlesticks.GetRange(startIndex, i - startIndex + 1);
                var highestHigh = group.Max(c => c.HighPrice);

                foreach (var candlestick in group)
                {
                    candlestick.Highests.Add(new Highest(candlestick.PrimaryId)
                    {
                        Period = period,
                        Value = highestHigh,
                        PriceType = PriceType.High
                    });
                }
            }
        }

        private static void CalculateHighestClose(PairExtended pair)
        {
            const int period = 22;
            int count = pair.Candlesticks.Count;

            for (int i = period - 1; i < count; i++) // Start from the 22nd candlestick, as we need to calculate over the last 22 candlesticks
            {
                int startIndex = i - (period - 1);
                var group = pair.Candlesticks.GetRange(startIndex, period);
                var highestClose = group.Max(c => c.ClosePrice);

                foreach (var candlestick in group)
                {
                    candlestick.Highests.Add(new Highest(candlestick.PrimaryId)
                    {
                        Period = period,
                        Value = highestClose,
                        PriceType = PriceType.Close
                    });
                }
            }
        }

        private static void CalculateFractalLowestHigh(PairExtended pair)
        {
            var candlesticksWithBearFractals = pair.Candlesticks.Where(c => c.Fractals.Count > 0 && c.Fractals.FirstOrDefault(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2) is not null)
                                                                .OrderBy(c => c.OpenDate).ToList();

            int count = 1;
            for (int i = 0; i < candlesticksWithBearFractals.Count; i++)
            {
                var candlestick = candlesticksWithBearFractals[i];
                var candlestick1 = i - 1 >= 0 ? candlesticksWithBearFractals[i - 1] : null;

                if (candlestick1 is null)
                {
                    continue;
                }

                var fractalBear = candlestick.Fractals?.FirstOrDefault(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2);
                var fractalBear1 = candlestick1.Fractals?.FirstOrDefault(f => f.FractalType == FractalType.BearFractal && f.WindowPeriod == 2);

                if (fractalBear?.Value <= fractalBear1?.Value)
                {
                    count++;
                    var candlestickInPair = pair.Candlesticks.Find(c => c.OpenDate == candlestick.OpenDate);
                    candlestickInPair?.FractalLowests.Add(new FractalLowest(candlestick.PrimaryId, count, PriceType.High, candlestick.HighPrice));
                }
                else
                {
                    count = 1;
                }
            }
        }

        private static void CalculateFractalLowestLow(PairExtended pair)
        {
            var candlesticksWithBullFractals = pair.Candlesticks.Where(c => c.Fractals.Count > 0 && c.Fractals.FirstOrDefault(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2) is not null)
                                                                .OrderBy(c => c.OpenDate).ToList();

            int count = 0;
            for (int i = 0; i < candlesticksWithBullFractals.Count; i++)
            {
                var candlestick = candlesticksWithBullFractals[i];
                var candlestick1 = i - 1 >= 0 ? candlesticksWithBullFractals[i - 1] : null;

                if (candlestick1 is null)
                {
                    continue;
                }

                var FractalBull = candlestick.Fractals?.FirstOrDefault(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2);
                var FractalBull1 = candlestick1.Fractals?.FirstOrDefault(f => f.FractalType == FractalType.BullFractal && f.WindowPeriod == 2);

                if (FractalBull?.Value <= FractalBull1?.Value)
                {
                    count++;
                    var candlestickInPair = pair.Candlesticks.FirstOrDefault(c => c.OpenDate == candlestick.OpenDate);
                    candlestickInPair?.FractalLowests.Add(new FractalLowest(candlestick.PrimaryId,
                        count,
                        PriceType.Low,
                        candlestick.LowPrice));
                }
                else
                {
                    count = 0;
                }
            }
        }

        private static void CalculateVolatility(PairExtended pair)
        {
            var stockData = new StockData(
                 pair.Candlesticks.Select(x => x.OpenPrice != null ? (double)x.OpenPrice : 0.0),
                 pair.Candlesticks.Select(x => x.HighPrice != null ? (double)x.HighPrice : 0.0),
                 pair.Candlesticks.Select(x => x.LowPrice != null ? (double)x.LowPrice : 0.0),
                 pair.Candlesticks.Select(x => x.ClosePrice != null ? (double)x.ClosePrice : 0.0),
                 pair.Candlesticks.Select(x => x.Volume != null ? (double)x.Volume : 0.0),
                 pair.Candlesticks.Select(x => x.CloseDate));

            const int period = 21;

            var results = stockData.CalculateHistoricalVolatilityPercentile(MovingAvgType.ExponentialMovingAverage, period, 252);

            var hvpValues = results.OutputValues["Hvp"];  // Retrieve the "Hvp" values list

            for (int counter = 0; counter < pair.Candlesticks.Count; counter++)
            {
                var candlestick = pair.Candlesticks[counter];
                double valueAtIndex = hvpValues.ElementAtOrDefault(counter);
                candlestick?.Volatilities.Add(Volatility.Create(candlestick.PrimaryId, valueAtIndex, period));
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
    }
}