namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public sealed class ProviderSynchronization
    {
        public ProviderPairAssetSyncInfo ProviderPairAssetSyncInfo { get; init; } = new();
        public List<ProviderCandlestickSyncInfo> CandlestickSyncInfos { get; init; } = [];
    }
}
