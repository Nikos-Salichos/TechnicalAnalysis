using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class PairStatisticsExtension
    {
        public static ILogger Logger { get; set; }

        public static void CalculatePairStatistics(this List<PairExtended> pairs)
        {
            CalculateAccumulatedCorrelationOfPairToAnotherPairCandlesticks(pairs);
        }

        private static void CalculateAccumulatedCorrelationOfPairToAnotherPairCandlesticks(List<PairExtended> pairs)
        {
            var preCalculationCandlesticks = pairs.ToDictionary(
                pair => pair.PrimaryId,
                pair => pair
                    .Candlesticks
                    .OrderBy(c => c.CloseDate)
                    .Select(c => c.ClosePrice)
                    .Where(d => d.HasValue)
                    .Select(d => (double)d.Value)
                    .ToArray()
            );

            var candlestickLength = pairs.ToDictionary(pair => pair.PrimaryId, pair => pair.Candlesticks.Count);

            Parallel.ForEach(pairs, ParallelConfig.GetOptions(), pair =>
            {
                int currentPairLength = candlestickLength[pair.PrimaryId];
                if (currentPairLength == 0)
                {
                    return;
                }

                var currentPairCandles = preCalculationCandlesticks[pair.PrimaryId];

                foreach (var correlatedPair in pairs)
                {
                    if (correlatedPair.PrimaryId == pair.PrimaryId)
                    {
                        continue;
                    }

                    var otherPairCandles = preCalculationCandlesticks[correlatedPair.PrimaryId];
                    int desiredLength = Math.Min(otherPairCandles.Length, currentPairLength);

                    for (int i = 0; i < desiredLength; i++)
                    {
                        var currentCandlestickSubset = new ArraySegment<double>(currentPairCandles, 0, i + 1);
                        var currentOtherPairCandlestickSubset = new ArraySegment<double>(otherPairCandles, 0, i + 1);

                        var correlation = Correlation.Pearson(currentOtherPairCandlestickSubset, currentCandlestickSubset);
                        pair.Candlesticks[i].CorrelationPerPair.TryAdd(correlatedPair.BaseAssetName, correlation);
                    }
                }
            });
        }

    }
}
