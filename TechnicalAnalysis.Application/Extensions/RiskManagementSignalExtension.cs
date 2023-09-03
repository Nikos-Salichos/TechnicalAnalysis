using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Indicators.Advanced;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class RiskManagementSignalExtension
    {
        public static List<Position> AverageDownStrategyCloseOneByOne(this List<PairExtended> pairs)
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
                    if (positionFound is not null && positionFound.OpenPosition && candlestick1.AverageTrueRanges.FirstOrDefault() is not null)
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

        public static List<Position> AverageDownStrategyCloseAll(this List<PairExtended> pairs)
        {
            List<Position> positions = new List<Position>();
            foreach (PairExtended pair in pairs)
            {
                _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();
                long positionId = 0;
                bool openPosition = false;
                foreach (var candlestick in pair.Candlesticks)
                {
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
                    if (positionFound is not null && positionFound.OpenPosition)
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
