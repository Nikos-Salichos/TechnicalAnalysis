namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class ProviderSynchronization
    {
        public ProviderPairAssetSyncInfo ProviderPairAssetSyncInfo { get; set; } = new ProviderPairAssetSyncInfo();
        public List<ProviderCandlestickSyncInfo> CandlestickSyncInfos { get; init; } = [];
    }
}
