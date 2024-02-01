using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class ProviderCandlestickSyncInfo : BaseEntity
    {
        public DataProvider DataProvider { get; init; }
        public Timeframe Timeframe { get; init; }
        public DateTime LastCandlestickSync { get; private set; }

        public void UpdateCandlestickInfo()
        {
            LastCandlestickSync = DateTime.UtcNow;
        }
    }
}
