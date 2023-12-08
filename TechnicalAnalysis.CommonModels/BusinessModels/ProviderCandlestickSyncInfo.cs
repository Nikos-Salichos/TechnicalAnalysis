using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class ProviderCandlestickSyncInfo : BaseEntity
    {
        public DataProvider DataProvider { get; init; }
        public Timeframe Timeframe { get; set; }
        public DateTime LastCandlestickSync { get; set; }

        public static ProviderCandlestickSyncInfo Create(DataProvider dataProvider, Timeframe timeframe, DateTime lastCandlestickSync)
            => new()
            {
                DataProvider = dataProvider,
                Timeframe = timeframe,
                LastCandlestickSync = lastCandlestickSync
            };
    }
}
