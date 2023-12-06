using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class ProviderSynchronization
    {
        public DataProvider DataProvider { get; set; }
        public ProviderPairAssetSyncInfo ProviderPairAssetSyncInfo { get; set; }
        public List<ProviderCandlestickSyncInfo> CandlestickSyncInfos { get; set; } = [];

        public ProviderSynchronization(DataProvider dataProvider)
        {
            DataProvider = dataProvider;
            ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo(DataProvider);
        }

        public ProviderSynchronization()
        {

        }
    }
}
