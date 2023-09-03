namespace TechnicalAnalysis.CommonModels.BusinessModels;

public class MarketStatistic
{
    public int NumberOfPairs { get; init; }
    public int NumberOfPairsWithEnhancedScanIsBuy { get; init; }
    public double PercentageOfPairsWithEnhancedScanIsBuy
    {
        get
        {
            return NumberOfPairs == 0
                ? 0
                : (double)NumberOfPairsWithEnhancedScanIsBuy / NumberOfPairs * 100;
        }
    }
}