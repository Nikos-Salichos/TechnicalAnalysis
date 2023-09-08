using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
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

        public static void CalculateAccumulatedCorrelationOfPairToAnotherPairCandlesticks(List<PairExtended> pairs)
        {
            var preCalcCandlesticks = pairs.ToImmutableDictionary(
                pair => pair.PrimaryId,
                pair => pair.Candlesticks.OrderBy(c => c.CloseDate).Select(c => c.ClosePrice).Where(d => d.HasValue).Select(d => (double)d.Value).ToList()
            );

            Parallel.ForEach(pairs, ParallelOption.GetOptions(), pair =>
            {
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

                if (!pair.Candlesticks.Any())
                {
                    return;
                }

                var currentPairOrderedCandles = preCalcCandlesticks[pair.PrimaryId];

                foreach (var correlatedPair in pairs)
                {
                    if (correlatedPair.PrimaryId == pair?.PrimaryId)
                    {
                        continue;
                    }

                    var otherPairOrderedCandles = preCalcCandlesticks[correlatedPair.PrimaryId];

                    int desiredLength = Math.Min(otherPairOrderedCandles.Count, currentPairOrderedCandles.Count);

                    List<double> closeOfCandlesticksInDouble = currentPairOrderedCandles.Take(desiredLength).ToList();
                    List<double> pairCorrelatedCloseCandlesticks = otherPairOrderedCandles.Take(desiredLength).ToList();

                    for (int i = 0; i < closeOfCandlesticksInDouble.Count; i++)
                    {
                        var currentCandlestickSubset = closeOfCandlesticksInDouble.Take(i + 1).ToList();
                        var currentOtherPairCandlestickSubset = pairCorrelatedCloseCandlesticks.Take(i + 1).ToList();

                        var correlation = Correlation.Pearson(currentOtherPairCandlestickSubset, currentCandlestickSubset);

                        pair?.Candlesticks.ElementAtOrDefault(i)?.CorrelationPerPair.TryAdd(correlatedPair.BaseAssetName, correlation);
                    }
                }
            });

        }

    }
}
