using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Indicators.Advanced;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class RiskManagementSignalExtension
    {
        public static IEnumerable<Position> AverageDownStrategyCloseOneByOne(this IEnumerable<PairExtended> pairs)
        {
            List<Position> positions = new List<Position>();
            foreach (PairExtended pair in pairs)
            {
                _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
                bool openPosition = false;
                for (int i = 1; i < pair.Candlesticks.Count; i++)
                {
                    var candlestick = pair.Candlesticks[i];
                    var candlestick1 = pair.Candlesticks[i - 1];

                    if (candlestick1.EnhancedScans.FirstOrDefault()?.EnhancedScanIsBuy != null && !openPosition)
                    {
                        Position position = new Position();
                        position.OpenPositionDate = candlestick.OpenDate;
                        position.EntryPositionPrice = candlestick.OpenPrice;
                        position.SignalType = nameof(EnhancedScan);
                        position.OpenPosition = true;
                        positions.Add(position);

                        openPosition = true;
                    }

                    var positionFound = positions.LastOrDefault();
                    if (positionFound?.OpenPosition == true && candlestick1.AverageTrueRanges.FirstOrDefault() is not null)
                    {
                        var percentage = candlestick1.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                        var thresholdValue = positionFound?.EntryPositionPrice * percentage * 2;
                        var pricePercentageBelowEntry = positionFound?.EntryPositionPrice - thresholdValue;
                        if (candlestick1.EnhancedScans.FirstOrDefault()?.EnhancedScanIsBuy != null
                            && openPosition
                            && candlestick1.ClosePrice <= pricePercentageBelowEntry)
                        {
                            Position position = new Position();
                            position.OpenPositionDate = candlestick.OpenDate;
                            position.EntryPositionPrice = candlestick.ClosePrice;
                            position.SignalType = nameof(EnhancedScan);
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

            }
            return positions;
        }

        public static IEnumerable<Position> AverageDownStrategyCloseAllBasedInFractalBreak(this IEnumerable<PairExtended> pairs)
        {
            List<Position> positions = new List<Position>();
            foreach (PairExtended pair in pairs)
            {
                _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
                long positionId = 0;
                bool openPosition = false;
                decimal? latestFractalPrice = null;
                for (int i = 0; i < pair.Candlesticks.Count; i++)
                {
                    CandlestickExtended? candlestick = pair.Candlesticks[i];

                    if (candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsBuy != null && !openPosition)
                    {
                        Position position = new Position();
                        position.OpenPositionDate = candlestick.OpenDate;
                        position.EntryPositionPrice = candlestick.ClosePrice;
                        position.SignalType = nameof(EnhancedScan);
                        position.OpenPosition = true;
                        positions.Add(position);

                        openPosition = true;
                        positionId = position.Id;
                        continue;
                    }

                    var positionFound = positions.LastOrDefault();
                    if (positionFound?.OpenPosition == true)
                    {
                        var percentage = candlestick.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                        var thresholdValue = positionFound?.EntryPositionPrice * percentage * 2;
                        var pricePercentageBelowEntry = positionFound?.EntryPositionPrice - thresholdValue;
                        if (candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsBuy != null
                            && openPosition
                            && candlestick.ClosePrice <= pricePercentageBelowEntry)
                        {
                            Position position = new Position();
                            position.OpenPositionDate = candlestick.OpenDate;
                            position.EntryPositionPrice = candlestick.ClosePrice;
                            position.SignalType = nameof(EnhancedScan);
                            positions.Add(position);
                            position.OpenPosition = true;

                            openPosition = true;
                            positionId = position.Id;
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

                        // openPosition = TakeProfitBasedInAtrPercentage(openPosition, candlestick, openPositions);
                        if (latestFractalPrice is null)
                        {
                            continue;
                        }

                        decimal? allPositionsProfitAndLoss = 0;
                        decimal? totalEntryPrice = 0;
                        foreach (var position in openPositions)
                        {
                            var positionProfitAndLoss = candlestick.ClosePrice - position.EntryPositionPrice;
                            allPositionsProfitAndLoss += positionProfitAndLoss;
                            totalEntryPrice += position.EntryPositionPrice;
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
            }
            return positions;
        }

        public static IEnumerable<Position> AverageDownStrategyCloseAllTakeProfitBasedOnAtr(this IEnumerable<PairExtended> pairs)
        {
            List<Position> positions = new List<Position>();
            foreach (PairExtended pair in pairs)
            {
                _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
                long positionId = 0;
                bool openPosition = false;
                decimal? latestFractalPrice = null;
                for (int i = 0; i < pair.Candlesticks.Count; i++)
                {
                    CandlestickExtended? candlestick = pair.Candlesticks[i];

                    if (candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsBuy != null && !openPosition)
                    {
                        Position position = new Position();
                        position.OpenPositionDate = candlestick.OpenDate;
                        position.EntryPositionPrice = candlestick.ClosePrice;
                        position.SignalType = nameof(EnhancedScan);
                        position.OpenPosition = true;
                        positions.Add(position);

                        openPosition = true;
                        positionId = position.Id;
                        continue;
                    }

                    var positionFound = positions.LastOrDefault();
                    if (positionFound?.OpenPosition == true)
                    {
                        var percentage = candlestick.AverageTrueRanges.FirstOrDefault()?.AverageTrueRangePercent / 100m;
                        var thresholdValue = positionFound?.EntryPositionPrice * percentage * 2;
                        var pricePercentageBelowEntry = positionFound?.EntryPositionPrice - thresholdValue;
                        if (candlestick.EnhancedScans.FirstOrDefault()?.EnhancedScanIsBuy != null
                            && openPosition
                            && candlestick.ClosePrice <= pricePercentageBelowEntry)
                        {
                            Position position = new Position();
                            position.OpenPositionDate = candlestick.OpenDate;
                            position.EntryPositionPrice = candlestick.ClosePrice;
                            position.SignalType = nameof(EnhancedScan);
                            positions.Add(position);
                            position.OpenPosition = true;

                            openPosition = true;
                            positionId = position.Id;
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
                        foreach (var position in openPositions)
                        {
                            var positionProfitAndLoss = candlestick.ClosePrice - position.EntryPositionPrice;
                            allPositionsProfitAndLoss += positionProfitAndLoss;
                            totalEntryPrice += position.EntryPositionPrice;
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
            }
            return positions;
        }

    }
}
