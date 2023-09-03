using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class PairExtension
    {
        public static IList<BinancePair> GetDollarPairs(IList<BinanceAsset> assets, IList<BinancePair> fetchedPairs)
        {
            var usdt = assets.FirstOrDefault(a => string.Equals(a.Asset, Constants.Usdt, StringComparison.InvariantCultureIgnoreCase));
            var usdc = assets.FirstOrDefault(a => string.Equals(a.Asset, Constants.Usdc, StringComparison.InvariantCultureIgnoreCase));
            var dai = assets.FirstOrDefault(a => string.Equals(a.Asset, Constants.Dai, StringComparison.InvariantCultureIgnoreCase));
            var busd = assets.FirstOrDefault(a => string.Equals(a.Asset, Constants.Busd, StringComparison.InvariantCultureIgnoreCase));

            var uniquePairsList = fetchedPairs.Where(fetchedPair => fetchedPair.QuoteAssetId == usdt?.Id).ToList();
            var baseAssetIds = new HashSet<long>(uniquePairsList.Select(p => p.BaseAssetId));
            uniquePairsList.AddRange(fetchedPairs.Where(fetchedPair => fetchedPair.QuoteAssetId == usdc?.Id && baseAssetIds.Add(fetchedPair.BaseAssetId)));
            uniquePairsList.AddRange(fetchedPairs.Where(fetchedPair => fetchedPair.QuoteAssetId == dai?.Id && baseAssetIds.Add(fetchedPair.BaseAssetId)));
            uniquePairsList.AddRange(fetchedPairs.Where(fetchedPair => fetchedPair.QuoteAssetId == busd?.Id && baseAssetIds.Add(fetchedPair.BaseAssetId)));

            return uniquePairsList;
        }

        public static IEnumerable<PairExtended> MapPairsToAssets(this IEnumerable<PairExtended> pairs, IEnumerable<Asset> assets)
        {
            foreach (var pair in pairs)
            {
                var asset0Id = assets.FirstOrDefault(a => a.PrimaryId == pair.BaseAssetId);
                var asset1Id = assets.FirstOrDefault(a => a.PrimaryId == pair.QuoteAssetId);
                if (asset0Id != null)
                {
                    pair.BaseAssetId = asset0Id.PrimaryId;
                    pair.BaseAssetName = asset0Id.Symbol;
                }
                if (asset1Id != null)
                {
                    pair.QuoteAssetId = asset1Id.PrimaryId;
                    pair.QuoteAssetName = asset1Id.Symbol;
                }
                if (!string.IsNullOrWhiteSpace(pair.QuoteAssetName))
                {
                    pair.Symbol = pair.BaseAssetName + "-" + pair.QuoteAssetName;
                }
                else
                {
                    pair.Symbol = pair.BaseAssetName;
                }
            }
            return pairs;
        }

        public static IEnumerable<PairExtended> MapPairsToCandlesticks(this IEnumerable<PairExtended> pairs, IEnumerable<CandlestickExtended> candlesticks)
        {
            var candlestickDict = candlesticks.ToLookup(c => c.PoolOrPairId);
            foreach (var pair in pairs.Where(pair => candlestickDict.Contains(pair.PrimaryId)))
            {
                pair.Candlesticks.AddRange(candlestickDict[pair.PrimaryId]);
            }
            return pairs.ToList();
        }

        public static void FindNewCandlesticks(this IEnumerable<BinancePair> pairs, IEnumerable<BinancePair> pairsWithExistingCandles)
        {
            foreach (var pair in pairs)
            {
                var existingPair = pairsWithExistingCandles.FirstOrDefault(p => p.Pair == pair.Pair);

                if (existingPair != null)
                {
                    pair.BinanceCandlesticks = pair.BinanceCandlesticks
                        .Where(binanceCandlestick => !existingPair.BinanceCandlesticks.Any(existingCandlestick =>
                            binanceCandlestick.OpenTime.EqualsYearMonthDayHourMinute(existingCandlestick.OpenTime)
                         && binanceCandlestick.CloseTime.EqualsYearMonthDayHourMinute(existingCandlestick.CloseTime)))
                        .ToList();
                }
            }
        }
    }
}
