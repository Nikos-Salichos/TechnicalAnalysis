namespace TechnicalAnalysis.CommonModels.BusinessModels;

public class MarketStatistic
{
    public SortedDictionary<DateTime, DailyStatistic> DailyStatistics { get; set; } = [];

    public void CalculateAndFilterPercentages(double thresholdPercentage)
    {
        var keysToRemove = new List<DateTime>();

        foreach (var kvp in DailyStatistics)
        {
            var dailyStats = kvp.Value;
            double percentage = dailyStats.PairsWithEnhancedScan.Count / (double)dailyStats.NumberOfPairs * 100;

            if (percentage < thresholdPercentage)
            {
                keysToRemove.Add(kvp.Key);
            }
            else
            {
                dailyStats.Percentage = percentage;
            }
        }

        foreach (var key in keysToRemove)
        {
            DailyStatistics.Remove(key);
        }
    }
}