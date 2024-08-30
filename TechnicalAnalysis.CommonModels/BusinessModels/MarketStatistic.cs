namespace TechnicalAnalysis.CommonModels.BusinessModels;

public class MarketStatistic
{
    public SortedDictionary<DateTime, DailyStatistic> DailyStatistics { get; set; } = [];

    public void CalculateAndFilterPercentages(double thresholdPercentage)
    {
        var keysToRemove = new List<DateTime>();

        foreach (var (key, dailyStats) in DailyStatistics)
        {
            double percentage = dailyStats.PairsWithEnhancedScan.Count / (double)dailyStats.NumberOfPairs * 100;

            if (percentage >= thresholdPercentage)
            {
                dailyStats.Percentage = percentage;
            }
            else
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            DailyStatistics.Remove(key);
        }
    }
}