using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class ProviderSynchronization
    {
        public DataProvider DataProvider { get; init; }
        public ProviderPairAssetSyncInfo ProviderPairAssetSyncInfo { get; set; } = new ProviderPairAssetSyncInfo();
        public List<ProviderCandlestickSyncInfo> CandlestickSyncInfos { get; set; } = [];
    }
}
