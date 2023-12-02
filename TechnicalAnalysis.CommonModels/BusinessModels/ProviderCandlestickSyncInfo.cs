using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class ProviderCandlestickSyncInfo : BaseEntity
    {
        public DataProvider DataProvider { get; set; }
        public Timeframe Timeframe { get; set; }
        public DateTime LastCandlestickSync { get; set; }

        public ProviderCandlestickSyncInfo(DataProvider dataProvider)
        {
            DataProvider = dataProvider;
        }
    }
}
