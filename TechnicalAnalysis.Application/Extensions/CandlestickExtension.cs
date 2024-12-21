using System.Globalization;
using System.Reflection;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class CandlestickExtension
    {
        public static void ParseCandlestickData(BinanceCandlestick candlestick, object? cell, PropertyInfo property)
        {
            cell ??= string.Empty;

            if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
            {
                if (decimal.TryParse(cell.ToString()?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                {
                    value = decimal.Round(value, 8);
                    property.SetValue(candlestick, value);
                }
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                var cellValue = cell.ToString();

                if (long.TryParse(cellValue, out var unixTimeMs))
                {
                    var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMs).DateTime;
                    property.SetValue(candlestick, dateTime);
                }
                else if (DateTime.TryParse(cellValue, out var dateTime))
                {
                    property.SetValue(candlestick, dateTime);
                }
            }
            else if ((property.PropertyType == typeof(long) || property.PropertyType == typeof(long?)) && long.TryParse(cell.ToString(), out var value))
            {
                property.SetValue(candlestick, value);
            }
        }

        public static (List<DateTime> MissingCandles, List<BinancePair> MissingSymbols) FindMissingCandles(this List<BinancePair> pairs)
        {
            var missingDates = new List<DateTime>();
            var missingSymbols = new List<BinancePair>();

            foreach (var pair in pairs.Where(static s => s.IsActive))
            {
                var candles = pair.BinanceCandlesticks.OrderBy(c => c.OpenTime);
                BinanceCandlestick? previousCandle = null;
                foreach (var currentCandle in candles)
                {
                    if (previousCandle != null)
                    {
                        var difference = currentCandle.OpenTime - previousCandle.CloseTime;

                        if (difference > TimeSpan.FromMinutes(1))
                        {
                            var startRange = previousCandle.OpenTime.AddDays(1);
                            var endRange = currentCandle.CloseTime.AddDays(-1);
                            for (var date = startRange; date <= endRange; date = date.AddDays(1))
                            {
                                missingDates.Add(date);
                            }
                        }
                    }
                    previousCandle = currentCandle;
                }

                if (missingDates.Count > 0)
                {
                    missingSymbols.Add(pair);
                }
            }

            return (MissingCandles: missingDates, MissingSymbols: missingSymbols);
        }

        public static void FillMissingDatesInDays(this List<BinancePair> pairs)
        {
            foreach (var pair in pairs.Select(p => p.BinanceCandlesticks))
            {
                for (int i = 1; i < pair.Count; i++)
                {
                    BinanceCandlestick currentCandle = pair[i];
                    BinanceCandlestick previousCandle = pair[i - 1];
                    TimeSpan timeBetweenCandles = currentCandle.OpenTime - previousCandle.CloseTime;
                    if (timeBetweenCandles > TimeSpan.FromDays(1))
                    {
                        // Add missing candles to list
                        DateTime missingCandleTime = previousCandle.CloseTime.AddDays(1);
                        while (missingCandleTime < currentCandle.OpenTime)
                        {
                            DateTime newOpenTime = missingCandleTime;
                            DateTime newCloseTime = newOpenTime.AddDays(1).AddTicks(-1);
                            BinanceCandlestick newCandlestick = new()
                            {
                                OpenTime = newOpenTime,
                                CloseTime = newCloseTime
                            };
                            pair.Add(newCandlestick);
                            missingCandleTime = missingCandleTime.AddDays(1); // Update the missingCandleTime
                        }
                    }
                }
            }
        }

        public static Timeframe ToTimeFrame(this string? period)
        {
            if (string.IsNullOrWhiteSpace(period))
            {
                throw new ArgumentException($"Invalid period: {period}");
            }

            return period.ToLowerInvariant() switch
            {
                "1d" => Timeframe.Daily,
                "1w" => Timeframe.Weekly,
                "1h" => Timeframe.OneHour,
                _ => Timeframe.None
            };
        }

        public static ValueClassificationType ToValueClassificationType(this string? valueClassification)
        {
            if (string.IsNullOrWhiteSpace(valueClassification))
            {
                throw new ArgumentException($"Invalid valueClassification: {valueClassification}");
            }

            valueClassification = new string(valueClassification
                    .Where(c => !char.IsWhiteSpace(c))
                    .ToArray())
                .ToUpperInvariant();

            return valueClassification switch
            {
                "EXTREMEFEAR" => ValueClassificationType.ExtremeFear,
                "FEAR" => ValueClassificationType.Fear,
                "NEUTRAL" => ValueClassificationType.Neutral,
                "GREED" => ValueClassificationType.Greed,
                "EXTREMEGREED" => ValueClassificationType.ExtremeGreed,
                _ => ValueClassificationType.None
            };
        }
    }
}