namespace TechnicalAnalysis.CommonModels.BusinessModels;

public class MarketStatistic
{
    public double NumberOfPairs { get; init; }

    public SortedDictionary<DateTime, HashSet<PairExtended>> NumberOfPairsEnhancedScanPerDate { get; set; } = [];

    public IDictionary<DateTime, double> CalculateAllPercentages()
    {
        var percentages = new Dictionary<DateTime, double>(NumberOfPairsEnhancedScanPerDate.Count);
        foreach (var (date, pairs) in NumberOfPairsEnhancedScanPerDate)
        {
            var percentage = pairs.Count / NumberOfPairs * 100;
            percentages.Add(date, percentage);
        }

        return percentages;
    }
}