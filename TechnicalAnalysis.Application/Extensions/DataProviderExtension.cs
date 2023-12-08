using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class DataProviderExtension
    {
        public static bool IsProviderSyncedToday(this ProviderSynchronization provider, Timeframe timeframe)
        {
            if (provider?.ProviderPairAssetSyncInfo?.LastAssetSync.Date == DateTime.UtcNow.Date
                && provider?.ProviderPairAssetSyncInfo?.LastPairSync.Date == DateTime.UtcNow.Date
                && provider.CandlestickSyncInfos.Any(candlestickSyncInfo =>
                    candlestickSyncInfo?.LastCandlestickSync.Date == DateTime.UtcNow.Date
                    && candlestickSyncInfo.Timeframe == timeframe
                    && timeframe == Timeframe.Daily))
            {
                return true;
            }

            DateTime currentDate = DateTime.UtcNow;
            DayOfWeek currentDayOfWeek = currentDate.DayOfWeek;

            int dayDifference = (currentDayOfWeek - DayOfWeek.Monday + 7) % 7;

            if (provider?.ProviderPairAssetSyncInfo?.LastAssetSync.Date == DateTime.UtcNow.Date
            && provider?.ProviderPairAssetSyncInfo?.LastPairSync.Date == DateTime.UtcNow.Date
            && provider.CandlestickSyncInfos.Any(candlestickSyncInfo =>
                    candlestickSyncInfo?.LastCandlestickSync.Date == DateTime.UtcNow.Date
                    && dayDifference > 0
                    && candlestickSyncInfo.Timeframe == timeframe
                    && timeframe == Timeframe.Weekly))
            {
                return true;
            }

            return false;
        }

        public static void UpdateProviderInfo(this ProviderSynchronization provider)
        {
            if (provider is null)
            {
                return;
            }
            var currentTime = DateTime.UtcNow;
            provider.ProviderPairAssetSyncInfo.LastAssetSync = currentTime;
            provider.ProviderPairAssetSyncInfo.LastPairSync = currentTime;
        }

        public static ProviderCandlestickSyncInfo GetOrCreateProviderCandlestickSyncInfo(this ProviderSynchronization providerSynchronization, DataProvider provider, Timeframe timeframe)
        {
            var providerCandlestickSyncInfoProviderFound = providerSynchronization.CandlestickSyncInfos
                .Find(c => c.DataProvider == provider && c.Timeframe == timeframe);

            if (providerCandlestickSyncInfoProviderFound is null)
            {
                var newProviderCandlestickSyncInfo = ProviderCandlestickSyncInfo.Create(providerSynchronization.DataProvider, timeframe, DateTime.UtcNow);
                providerSynchronization.CandlestickSyncInfos.Add(newProviderCandlestickSyncInfo);
                return newProviderCandlestickSyncInfo;
            }
            else
            {
                providerCandlestickSyncInfoProviderFound.LastCandlestickSync = DateTime.UtcNow;
                return providerCandlestickSyncInfoProviderFound;
            }
        }

    }
}
