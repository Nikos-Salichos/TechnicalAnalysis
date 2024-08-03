using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class PairExtension
    {
        public static List<PairExtended> GetUniqueDollarPairs(List<Asset> assets, List<PairExtended> fetchedPairs)
        {
            // Retrieve the IDs for USDT, USDC, DAI, and BUSD from the assets
            var assetIds = assets
                .Where(a => string.Equals(a.Symbol, Constants.Usdt, StringComparison.InvariantCultureIgnoreCase) ||
                            string.Equals(a.Symbol, Constants.Usdc, StringComparison.InvariantCultureIgnoreCase) ||
                            string.Equals(a.Symbol, Constants.Dai, StringComparison.InvariantCultureIgnoreCase) ||
                            string.Equals(a.Symbol, Constants.Busd, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.PrimaryId)
                .ToHashSet();

            // Use a HashSet to keep track of unique BaseAssetIds
            var uniqueBaseAssetIds = new HashSet<long>();

            // Filter and add pairs to the list only if they haven't been added before
            return fetchedPairs
                .Where(fetchedPair => assetIds.Contains(fetchedPair.QuoteAssetId) &&
                                       uniqueBaseAssetIds.Add(fetchedPair.BaseAssetId)).ToList();
        }

        public static List<PairExtended> MapPairsToAssets(this List<PairExtended> pairs, List<Asset> assets)
        {
            foreach (var pair in pairs)
            {
                var asset0Id = assets.Find(a => a.PrimaryId == pair.BaseAssetId);
                var asset1Id = assets.Find(a => a.PrimaryId == pair.QuoteAssetId);
                if (asset0Id != null)
                {
                    pair.BaseAssetId = asset0Id.PrimaryId;
                    pair.BaseAssetName = asset0Id.Symbol;
                    pair.ProductType = asset0Id.ProductType;
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
            return pairs.ToList();
        }

        public static List<PairExtended> MapPairsToCandlesticks(this List<PairExtended> pairs, List<CandlestickExtended> candlesticks)
        {
            var candlestickDict = candlesticks.ToLookup(c => c.PoolOrPairId);
            foreach (var pair in pairs.Where(pair => candlestickDict.Contains(pair.PrimaryId)))
            {
                pair.Candlesticks.AddRange(candlestickDict[pair.PrimaryId]);
            }
            return pairs.ToList();
        }

        public static void FindNewCandlesticks(this List<BinancePair> pairs, List<BinancePair> pairsWithExistingCandles)
        {
            foreach (var pair in pairs)
            {
                var existingPair = pairsWithExistingCandles.Find(p => p.Pair == pair.Pair);

                if (existingPair != null)
                {
                    pair.BinanceCandlesticks = pair.BinanceCandlesticks
                        .Where(binanceCandlestick => !existingPair.BinanceCandlesticks.Exists(existingCandlestick =>
                            binanceCandlestick.OpenTime.EqualsYearMonthDayHourMinute(existingCandlestick.OpenTime)
                         && binanceCandlestick.CloseTime.EqualsYearMonthDayHourMinute(existingCandlestick.CloseTime)
                         && binanceCandlestick.Period == existingCandlestick.Period))
                        .ToList();
                }
            }
        }
    }
}
