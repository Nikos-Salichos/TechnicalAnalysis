using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Indicators.Advanced;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class RiskManagementSignalExtension
    {
        public static List<Position> AverageDownStrategyCloseOneByOne(this PairExtended pair)
        {
            List<Position> positions = new();

            _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
            bool openPosition = false;
            for (int i = 1; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = pair.Candlesticks[i - 1];

                if (candlestick1.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true && !openPosition)
                {
                    Position position = new()
                    {
                        OpenPositionDate = candlestick.OpenDate,
                        EntryPositionPrice = candlestick.OpenPrice,
                        SignalType = nameof(EnhancedScan),
                        OpenPosition = true
                    };
                    positions.Add(position);

                    openPosition = true;
                }

                var positionFound = positions.LastOrDefault();
                if (positionFound?.OpenPosition == true && candlestick1.AverageTrueRanges.Count is not 0)
                {
                    var percentage = candlestick1.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                    var thresholdValue = positionFound?.EntryPositionPrice * percentage * 2;
                    var pricePercentageBelowEntry = positionFound?.EntryPositionPrice - thresholdValue;
                    if (candlestick1.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true
                        && openPosition
                        && candlestick1.ClosePrice <= pricePercentageBelowEntry)
                    {
                        Position position = new()
                        {
                            OpenPositionDate = candlestick.OpenDate,
                            EntryPositionPrice = candlestick.ClosePrice,
                            SignalType = nameof(EnhancedScan)
                        };
                        positions.Add(position);
                        position.OpenPosition = true;

                        openPosition = true;
                    }
                }

                var openPositions = positions.Where(p => p.OpenPosition).ToList();

                if (openPositions.Count > 0)
                {
                    foreach (var position in openPositions)
                    {
                        if (candlestick1.AverageTrueRanges.FirstOrDefault() is null)
                        {
                            continue;
                        }
                        var percentage = candlestick1.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                        var thresholdValue = position?.EntryPositionPrice * percentage * 2;
                        var pricePercentageAboveEntry = position?.EntryPositionPrice + thresholdValue;
                        if (candlestick1.ClosePrice > pricePercentageAboveEntry)
                        {
                            position.ClosePositionPrice = candlestick.OpenPrice;
                            position.ClosePositionDate = candlestick.CloseDate;
                            position.OpenPosition = false;
                            openPosition = false;
                        }
                    }
                }
            }

            return positions;
        }

        public static List<Position> AverageDownStrategyCloseOneByOnBasedInFractalBreak(this PairExtended pair)
        {
            List<Position> positions = [];

            _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
            bool openPosition = false;
            decimal? latestFractalPrice = null;
            for (int i = 1; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = pair.Candlesticks[i - 1];

                if (candlestick1.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true && !openPosition)
                {
                    Position position = new Position
                    {
                        OpenPositionDate = candlestick.OpenDate,
                        EntryPositionPrice = candlestick.OpenPrice,
                        SignalType = nameof(EnhancedScan),
                        OpenPosition = true
                    };
                    positions.Add(position);

                    openPosition = true;
                }

                var positionFound = positions.LastOrDefault();
                if (positionFound?.OpenPosition == true && candlestick1.AverageTrueRanges.Count is not 0)
                {
                    var percentage = candlestick1.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                    var thresholdValue = positionFound?.EntryPositionPrice * percentage * 2;
                    var pricePercentageBelowEntry = positionFound?.EntryPositionPrice - thresholdValue;
                    if (candlestick1.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true
                        && openPosition
                        && candlestick1.ClosePrice <= pricePercentageBelowEntry)
                    {
                        Position position = new Position
                        {
                            OpenPositionDate = candlestick.OpenDate,
                            EntryPositionPrice = candlestick.ClosePrice,
                            SignalType = nameof(EnhancedScan)
                        };
                        positions.Add(position);
                        position.OpenPosition = true;

                        openPosition = true;
                    }
                }

                var openPositions = positions.Where(p => p.OpenPosition).ToList();
                if (openPositions.Count > 0)
                {
                    var candlestick3 = i - 2 >= 0 ? pair.Candlesticks[i - 2] : null;
                    if (candlestick3 is not null)
                    {
                        var latestFractal = candlestick3.Fractals.FirstOrDefault();
                        if (latestFractal?.FractalType == CommonModels.Enums.FractalType.BullFractal)
                        {
                            latestFractalPrice = latestFractal.Value;
                        }
                    }

                    if (latestFractalPrice is null)
                    {
                        continue;
                    }

                    if (candlestick.ClosePrice < latestFractalPrice.Value)
                    {
                        foreach (var position in openPositions)
                        {
                            if (candlestick.ClosePrice > position.EntryPositionPrice)
                            {
                                position.ClosePositionPrice = candlestick.ClosePrice;
                                position.ClosePositionDate = candlestick.CloseDate;
                                position.OpenPosition = false;
                                openPosition = false;
                            }
                            latestFractalPrice = null;
                        }
                    }
                }
            }

            return positions;
        }

        public static List<Position> AverageDownStrategyCloseAllBasedInFractalBreak(this PairExtended pair)
        {
            List<Position> positions = new();

            _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
            bool openPosition = false;
            decimal? latestFractalPrice = null;
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                CandlestickExtended? candlestick = pair.Candlesticks[i];

                if (candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true && !openPosition)
                {
                    Position position = new Position
                    {
                        OpenPositionDate = candlestick.OpenDate,
                        EntryPositionPrice = candlestick.ClosePrice,
                        SignalType = nameof(EnhancedScan),
                        OpenPosition = true
                    };
                    positions.Add(position);

                    openPosition = true;
                    continue;
                }

                var positionFound = positions.LastOrDefault();
                if (positionFound?.OpenPosition == true)
                {
                    var percentage = candlestick.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                    var thresholdValue = positionFound.EntryPositionPrice * percentage;
                    var pricePercentageBelowEntry = positionFound.EntryPositionPrice - thresholdValue;
                    if (candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true
                        && openPosition
                        && candlestick.ClosePrice <= pricePercentageBelowEntry)
                    {
                        Position position = new Position
                        {
                            OpenPositionDate = candlestick.OpenDate,
                            EntryPositionPrice = candlestick.ClosePrice,
                            SignalType = nameof(EnhancedScan)
                        };
                        positions.Add(position);
                        position.OpenPosition = true;

                        openPosition = true;
                        continue;
                    }
                }

                var openPositions = positions.Where(p => p.OpenPosition).ToList();
                if (openPositions.Count > 0)
                {
                    var candlestick3 = i - 2 >= 0 ? pair.Candlesticks[i - 2] : null;
                    if (candlestick3 is not null)
                    {
                        var latestFractal = candlestick3.Fractals.FirstOrDefault();
                        if (latestFractal?.FractalType == CommonModels.Enums.FractalType.BullFractal)
                        {
                            latestFractalPrice = latestFractal.Value;
                        }
                    }

                    if (latestFractalPrice is null)
                    {
                        continue;
                    }

                    decimal? allPositionsProfitAndLoss = 0;
                    decimal? totalEntryPrice = 0;
                    foreach (var entryPositionPrice in openPositions.Select(op => op.EntryPositionPrice))
                    {
                        var positionProfitAndLoss = candlestick.ClosePrice - entryPositionPrice;
                        allPositionsProfitAndLoss += positionProfitAndLoss;
                        totalEntryPrice += entryPositionPrice;
                    }

                    decimal? positionsAverageEntryPrice = totalEntryPrice / openPositions.Count;

                    if (candlestick.ClosePrice > positionsAverageEntryPrice && candlestick.ClosePrice < latestFractalPrice.Value)
                    {
                        foreach (var position in openPositions)
                        {
                            position.ClosePositionPrice = candlestick.ClosePrice;
                            position.ClosePositionDate = candlestick.CloseDate;
                            position.OpenPosition = false;
                            openPosition = false;
                        }
                        latestFractalPrice = null;
                    }
                }
            }

            var totalProfitAndLoss = positions.Sum(p => p.ClosedProfitOrLoss);

            return positions;
        }

        public static List<Position> AverageDownStrategyCloseAllTakeProfitBasedOnAtr(this PairExtended pair)
        {
            List<Position> positions = new List<Position>();

            _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
            bool openPosition = false;
            decimal? latestFractalPrice = null;
            for (int i = 0; i < pair.Candlesticks.Count; i++)
            {
                CandlestickExtended? candlestick = pair.Candlesticks[i];

                if (candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true && !openPosition)
                {
                    Position position = new Position
                    {
                        OpenPositionDate = candlestick.OpenDate,
                        EntryPositionPrice = candlestick.ClosePrice,
                        SignalType = nameof(EnhancedScan),
                        OpenPosition = true
                    };
                    positions.Add(position);

                    openPosition = true;
                    continue;
                }

                var positionFound = positions.LastOrDefault();
                if (positionFound?.OpenPosition == true)
                {
                    var percentage = candlestick.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                    var thresholdValue = positionFound.EntryPositionPrice * percentage * 2;
                    var pricePercentageBelowEntry = positionFound.EntryPositionPrice - thresholdValue;
                    if (candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true
                        && openPosition
                        && candlestick.ClosePrice <= pricePercentageBelowEntry)
                    {
                        Position position = new Position
                        {
                            OpenPositionDate = candlestick.OpenDate,
                            EntryPositionPrice = candlestick.ClosePrice,
                            SignalType = nameof(EnhancedScan)
                        };
                        positions.Add(position);
                        position.OpenPosition = true;

                        openPosition = true;
                        continue;
                    }
                }

                var openPositions = positions.Where(p => p.OpenPosition).ToList();

                if (openPositions.Count > 0)
                {
                    var candlestick3 = i - 2 >= 0 ? pair.Candlesticks[i - 2] : null;
                    if (candlestick3 is not null)
                    {
                        var latestFractal = candlestick3.Fractals.FirstOrDefault();
                        if (latestFractal?.FractalType == CommonModels.Enums.FractalType.BullFractal)
                        {
                            latestFractalPrice = latestFractal.Value;
                        }
                    }

                    decimal? allPositionsProfitAndLoss = 0;
                    decimal? totalEntryPrice = 0;
                    foreach (var entryPositionPrice in openPositions.Select(op => op.EntryPositionPrice))
                    {
                        var positionProfitAndLoss = candlestick.ClosePrice - entryPositionPrice;
                        allPositionsProfitAndLoss += positionProfitAndLoss;
                        totalEntryPrice += entryPositionPrice;
                    }

                    decimal? positionsAverageEntryPrice = totalEntryPrice / openPositions.Count;

                    var percentage = candlestick.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                    var thresholdValue = positionsAverageEntryPrice * percentage * 2;
                    var pricePercentageAboveEntry = positionsAverageEntryPrice + thresholdValue;
                    if (candlestick.ClosePrice > pricePercentageAboveEntry)
                    {
                        foreach (var position in openPositions)
                        {
                            position.ClosePositionPrice = candlestick.ClosePrice;
                            position.ClosePositionDate = candlestick.CloseDate;
                            position.OpenPosition = false;
                            openPosition = false;
                        }
                    }
                }
            }

            return positions;
        }

        public static List<Position> AverageDownStrategyCloseAllBasedOnEnhancedReversalSignal(this PairExtended pair)
        {
            List<Position> positions = [];
            _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
            bool openPosition = false;

            for (int i = 1; i < pair.Candlesticks.Count; i++)
            {
                var candlestick = pair.Candlesticks[i];
                var candlestick1 = pair.Candlesticks[i - 1];

                if (candlestick1.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true && !openPosition)
                {
                    Position position = new()
                    {
                        OpenPositionDate = candlestick.OpenDate,
                        EntryPositionPrice = candlestick.OpenPrice,
                        SignalType = nameof(EnhancedScan),
                        OpenPosition = true
                    };
                    positions.Add(position);

                    openPosition = true;
                }

                var positionFound = positions.LastOrDefault();
                if (positionFound?.OpenPosition == true && candlestick1.AverageTrueRanges.Count is not 0)
                {
                    var percentage = candlestick1.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                    var thresholdValue = positionFound?.EntryPositionPrice * percentage * 2;
                    var pricePercentageBelowEntry = positionFound?.EntryPositionPrice - thresholdValue;
                    if (candlestick1.EnhancedScans.FirstOrDefault()?.EnhancedScanIsLong is true
                        && openPosition
                        && candlestick1.ClosePrice <= pricePercentageBelowEntry)
                    {
                        Position position = new()
                        {
                            OpenPositionDate = candlestick.OpenDate,
                            EntryPositionPrice = candlestick.ClosePrice,
                            SignalType = nameof(EnhancedScan)
                        };
                        positions.Add(position);
                        position.OpenPosition = true;

                        openPosition = true;
                    }
                }

                var openPositions = positions.Where(p => p.OpenPosition).ToList();
                if (openPositions.Count > 0)
                {
                    var closeLongSignal = candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsShort;

                    if (candlestick.ClosePrice < candlestick1.ClosePrice.Value && closeLongSignal is true)
                    {
                        foreach (var position in openPositions)
                        {
                            if (candlestick.ClosePrice > position.EntryPositionPrice)
                            {
                                position.ClosePositionPrice = candlestick.ClosePrice;
                                position.ClosePositionDate = candlestick.CloseDate;
                                position.OpenPosition = false;
                                openPosition = false;
                            }
                        }
                    }
                }
            }

            return positions;
        }
    }
}
