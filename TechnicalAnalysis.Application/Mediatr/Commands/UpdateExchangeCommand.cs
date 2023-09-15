using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class UpdateExchangeCommand : IRequest
    {
        public ProviderPairAssetSyncInfo ProviderPairAssetSyncInfo { get; }
        public ProviderCandlestickSyncInfo ProviderCandlestickSyncInfo { get; }

        public UpdateExchangeCommand(ProviderPairAssetSyncInfo providerPairAssetSyncInfo,
            ProviderCandlestickSyncInfo providerCandlestickSyncInfo)
        {
            ProviderPairAssetSyncInfo = providerPairAssetSyncInfo;
            ProviderCandlestickSyncInfo = providerCandlestickSyncInfo;
        }
    }
}