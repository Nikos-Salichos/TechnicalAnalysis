using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Update
{
    public class UpdateExchangeCommand(
        ProviderPairAssetSyncInfo? providerPairAssetSyncInfo = null,
        ProviderCandlestickSyncInfo? providerCandlestickSyncInfo = null) : IRequest
    {
        public ProviderPairAssetSyncInfo? ProviderPairAssetSyncInfo { get; } = providerPairAssetSyncInfo;
        public ProviderCandlestickSyncInfo? ProviderCandlestickSyncInfo { get; } = providerCandlestickSyncInfo;
    }
}