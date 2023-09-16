using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Utilities;

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
            var preCalcCandlesticks = pairs.ToImmutableDictionary(
                pair => pair.PrimaryId,
                pair => pair
                    .Candlesticks
                    .OrderBy(c => c.CloseDate)
                    .Select(c => c.ClosePrice)
                    .Where(d => d.HasValue)
                    .Select(d => (double)d.Value)
                    .ToList()
            );

            // Outside of the loop
            ConcurrentDictionary<long, double[]> preCalcCandlestickArrays = new ConcurrentDictionary<long, double[]>();

            // Convert to arrays once, for faster access
            foreach (var preCalcCandlestick in preCalcCandlesticks)
            {
                preCalcCandlestickArrays[preCalcCandlestick.Key] = preCalcCandlestick.Value.ToArray();
            }

            // Main loop
            Parallel.ForEach(pairs, ParallelOption.GetOptions(), pair =>
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

                var currentPairOrderedCandles = preCalcCandlestickArrays[pair.PrimaryId];
                int currentPairOrderedCandlesLength = currentPairOrderedCandles.Length;

                foreach (var correlatedPair in pairs)
                {
                    if (correlatedPair.PrimaryId == pair.PrimaryId)
                    {
                        continue;
                    }

                    var otherPairOrderedCandles = preCalcCandlestickArrays[correlatedPair.PrimaryId];
                    int desiredLength = Math.Min(otherPairOrderedCandles.Length, currentPairOrderedCandlesLength);

                    for (int i = 0; i < desiredLength; i++)
                    {
                        var currentCandlestickSubset = currentPairOrderedCandles.Take(i + 1).ToArray();
                        var currentOtherPairCandlestickSubset = otherPairOrderedCandles.Take(i + 1).ToArray();

                        var correlation = Correlation.Pearson(currentOtherPairCandlestickSubset, currentCandlestickSubset);
                        pair.Candlesticks[i].CorrelationPerPair.TryAdd(correlatedPair.BaseAssetName, correlation);
                    }
                }
            });





        }



    }
}
