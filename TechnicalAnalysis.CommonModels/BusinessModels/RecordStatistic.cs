namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class DailyStatistic
    {
        public int NumberOfPairs { get; set; }
        public HashSet<string> PairsWithEnhancedScan { get; set; } = [];
        public double Percentage { get; set; }
    }
}
