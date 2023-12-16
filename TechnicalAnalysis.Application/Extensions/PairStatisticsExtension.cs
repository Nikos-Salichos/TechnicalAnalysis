using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class PairStatisticsExtension
    {
        public static ILogger Logger { get; set; }

        public static void CalculatePairStatistics(this IEnumerable<PairExtended> pairs)
        {
            CalculateAccumulatedCorrelationOfPairToAnotherPairCandlesticks(pairs);
        }

        private static void CalculateAccumulatedCorrelationOfPairToAnotherPairCandlesticks(IEnumerable<PairExtended> pairs)
        {
            var preCalculationCandlesticks = pairs.ToImmutableDictionary(
                pair => pair.PrimaryId,
                pair => pair
                    .Candlesticks
                    .OrderBy(c => c.CloseDate)
                    .Select(c => c.ClosePrice)
                    .Where(d => d.HasValue)
                    .Select(d => (double)d.Value)
                    .ToList()
            );

            foreach (var pair in pairs)
            {
                if (pair.Candlesticks.Count == 0)
                {
                    return;
                }

                Logger.LogInformation("Method name: {MethodName} - Pair details - {PairPropertyName}: {PairName}, " +
                "{BaseAssetContractPropertyName}: {BaseAssetContract}, " +
                "{BaseAssetNamePropertyName}: {BaseAssetName}, " +
                "{QuoteAssetContractPropertyName}: {QuoteAssetContract}, " +
                "{QuoteAssetNamePropertyName}: {QuoteAssetName}", nameof(CalculateAccumulatedCorrelationOfPairToAnotherPairCandlesticks),
                nameof(pair.Symbol), pair.Symbol,
                nameof(pair.BaseAssetContract), pair.BaseAssetContract,
                nameof(pair.BaseAssetName), pair.BaseAssetName,
                nameof(pair.QuoteAssetContract), pair.QuoteAssetContract,
                nameof(pair.QuoteAssetName), pair.QuoteAssetName);

                var currentPairOrderedCandles = preCalculationCandlesticks[pair.PrimaryId];
                int currentPairOrderedCandlesLength = currentPairOrderedCandles.Count;

                foreach (var correlatedPair in pairs)
                {
                    if (correlatedPair.PrimaryId == pair.PrimaryId)
                    {
                        continue;
                    }

                    var otherPairOrderedCandles = preCalculationCandlesticks[correlatedPair.PrimaryId];
                    int desiredLength = Math.Min(otherPairOrderedCandles.Count, currentPairOrderedCandlesLength);

                    for (int i = 0; i < desiredLength; i++)
                    {
                        var currentCandlestickSubset = currentPairOrderedCandles.Take(i + 1).ToArray();
                        var currentOtherPairCandlestickSubset = otherPairOrderedCandles.Take(i + 1).ToArray();

                        var correlation = Correlation.Pearson(currentOtherPairCandlestickSubset, currentCandlestickSubset);
                        pair.Candlesticks[i].CorrelationPerPair.TryAdd(correlatedPair.BaseAssetName, correlation);
                    }
                }
            }
        }

    }
}
