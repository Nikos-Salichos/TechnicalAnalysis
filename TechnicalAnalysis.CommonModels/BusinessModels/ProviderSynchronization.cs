using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class ProviderSynchronization
    {
        public DataProvider DataProvider { get; set; }
        public ProviderPairAssetSyncInfo ProviderPairAssetSyncInfo { get; set; } = new ProviderPairAssetSyncInfo();
        public List<ProviderCandlestickSyncInfo> CandlestickSyncInfos { get; set; } = [];

        public static ProviderSynchronization Create(DataProvider dataProvider)
            => new()
            {
                DataProvider = dataProvider
            };
    }
}
