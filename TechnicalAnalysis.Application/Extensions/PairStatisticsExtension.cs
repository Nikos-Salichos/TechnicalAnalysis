using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class PairStatisticsExtension
    {
        public static ILogger Logger { get; set; }

        public static void CalculatePairStatistics(this List<PairExtended> pairs)
        {
            foreach (var pair in pairs)
            {
                Logger.LogInformation("Method name: {MethodName} - Pair details - {PairPropertyName}: {PairName}, " +
                    "{PairPropertyContractAddress}: {PairContractAddress}, " +
                    "{BaseAssetContractPropertyName}: {BaseAssetContract}, " +
                    "{BaseAssetNamePropertyName}: {BaseAssetName}, " +
                    "{QuoteAssetContractPropertyName}: {QuoteAssetContract}, " +
                    "{QuoteAssetNamePropertyName}: {QuoteAssetName}", nameof(CalculatePairStatistics),
                nameof(pair.Symbol), pair.Symbol,
                nameof(pair.ContractAddress), pair.ContractAddress,
                nameof(pair.BaseAssetContract), pair.BaseAssetContract,
                nameof(pair.BaseAssetName), pair.BaseAssetName,
                nameof(pair.QuoteAssetContract), pair.QuoteAssetContract,
                nameof(pair.QuoteAssetName), pair.QuoteAssetName);

                pair.Candlesticks = pair.Candlesticks.OrderBy(c => c.CloseDate).ToList();

                CalculateAccumulatedCorrelationOfPairToAnotherPairCandlesticks(pairs, pair);
            }
        }

        public static void CalculateAccumulatedCorrelationOfPairToAnotherPairCandlesticks(List<PairExtended> pairs, PairExtended pair)
        {
            if (pair is null || pair.Candlesticks.Count == 0)
            {
                return;
            }

            foreach (var correlatedPair in pairs)
            {
                if (correlatedPair.PrimaryId == pair?.PrimaryId)
                {
                    continue;
                }

                var otherPairCandlesticks = correlatedPair.Candlesticks.OrderByDescending(c => c.CloseDate);
                var otherPairandlestickClosePrices = otherPairCandlesticks.Select(c => c.ClosePrice);

                var candlestickCloses = pair?.Candlesticks?.OrderByDescending(c => c.CloseDate).Select(c => c.ClosePrice);
                int desiredLength = Math.Min(otherPairandlestickClosePrices.Count(), candlestickCloses.Count());

                List<double> closeOfCandlesticksInDouble = candlestickCloses
                    .Where(d => d.HasValue)
                    .Select(d => (double)d.Value)
                    .Take(desiredLength).ToList();

                List<double> pairCorrelatedCloseCandlesticks = otherPairandlestickClosePrices
                    .Where(d => d.HasValue)
                    .Select(d => (double)d.Value)
                    .Take(desiredLength).ToList();

                for (int i = 0; i < closeOfCandlesticksInDouble.Count; i++)
                {
                    List<double> currentCandlestickSubset = closeOfCandlesticksInDouble.Take(i + 1).ToList();
                    List<double> currentBtcCandlestickSubset = pairCorrelatedCloseCandlesticks.Take(i + 1).ToList();

                    var correlation = Correlation.Pearson(currentBtcCandlestickSubset, currentCandlestickSubset);
                    pair?.Candlesticks?.ElementAtOrDefault(i)?.CorrelationPerPair.TryAdd(correlatedPair.BaseAssetName, correlation);
                }
            }
        }

    }
}
