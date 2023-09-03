using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Indicators.Advanced;
using TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class BacktestExtension
    {
        public static List<Position> RunBackTest(this PairExtended pair)
        {
            _ = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();

            List<Position> positions = new List<Position>();

            foreach (var candlestick in pair.Candlesticks)
            {
                if (candlestick.DragonFlyDojis.Count > 0 && candlestick.DragonFlyDojis[0].IsDragonFlyDoji)
                {
                    Position position = new Position();
                    position.OpenPositionDate = candlestick.OpenDate;
                    position.EntryPositionPrice = candlestick.ClosePrice;
                    position.OpenPosition = true;
                    position.SignalType = nameof(DragonFlyDoji);
                    positions.Add(position);
                }

                if (candlestick.InvertedHammers.Count > 0 && candlestick.InvertedHammers[0].IsInvertedHammer)
                {
                    Position position = new Position();
                    position.OpenPositionDate = candlestick.OpenDate;
                    position.EntryPositionPrice = candlestick.ClosePrice;
                    position.OpenPosition = true;
                    position.SignalType = nameof(InvertedHammer);
                    positions.Add(position);
                }

                if (candlestick.Hammers.Count > 0 && candlestick.Hammers[0].IsHammer)
                {
                    Position position = new Position();
                    position.OpenPositionDate = candlestick.OpenDate;
                    position.EntryPositionPrice = candlestick.ClosePrice;
                    position.OpenPosition = true;
                    position.SignalType = nameof(Hammer);
                    positions.Add(position);
                }

                if (candlestick.SpinningTops.Count > 0 && candlestick.SpinningTops[0].IsSpinningTop)
                {
                    Position position = new Position();
                    position.OpenPositionDate = candlestick.OpenDate;
                    position.EntryPositionPrice = candlestick.ClosePrice;
                    position.OpenPosition = true;
                    position.SignalType = nameof(SpinningTop);
                    positions.Add(position);
                }

                if (candlestick.EnhancedScans.Count > 0 && candlestick.EnhancedScans[0].EnhancedScanIsBuy)
                {
                    Position position = new Position();
                    position.OpenPositionDate = candlestick.OpenDate;
                    position.EntryPositionPrice = candlestick.ClosePrice;
                    position.OpenPosition = true;
                    position.SignalType = nameof(EnhancedScan);
                    positions.Add(position);
                }

                foreach (var position in positions)
                {
                    if (position.OpenPosition && candlestick.ClosePrice >= position.EntryPositionPrice + position.EntryPositionPrice * 0.1m)
                    {
                        position.ClosePositionDate = candlestick.OpenDate;
                        position.ClosePositionPrice = position.EntryPositionPrice + position.EntryPositionPrice * 0.1m;
                        position.OpenPosition = false;
                    }
                }
            }

            return positions;
        }
    }
}
