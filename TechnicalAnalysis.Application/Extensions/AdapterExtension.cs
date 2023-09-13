using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class AdapterExtension
    {
        public static bool IsProviderSyncedToday(this Provider provider, Timeframe timeframe)
        {
            return provider?.LastAssetSync.Date == DateTime.UtcNow.Date
                && provider?.LastPairSync.Date == DateTime.UtcNow.Date
                && provider.CandlestickSyncInfos.Any(candlestickSyncInfo =>
                    candlestickSyncInfo?.LastCandlestickSync.Date == DateTime.UtcNow.Date
                    && candlestickSyncInfo.Timeframe == timeframe);
        }
    }
}
