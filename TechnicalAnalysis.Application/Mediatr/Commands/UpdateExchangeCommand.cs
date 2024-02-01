using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class UpdateExchangeCommand(ProviderPairAssetSyncInfo providerPairAssetSyncInfo,
        ProviderCandlestickSyncInfo providerCandlestickSyncInfo) : IRequest
    {
        public ProviderPairAssetSyncInfo ProviderPairAssetSyncInfo { get; } = providerPairAssetSyncInfo;
        public ProviderCandlestickSyncInfo ProviderCandlestickSyncInfo { get; } = providerCandlestickSyncInfo;
    }
}