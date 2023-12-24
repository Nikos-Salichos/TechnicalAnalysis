using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class DataProviderExtension
    {
        public static bool IsProviderSyncedToday(this ProviderSynchronization provider, Timeframe timeframe)
        {
            if (provider is null)
            {
                return false;
            }

            if (provider.ProviderPairAssetSyncInfo.LastAssetSync.Date == DateTime.UtcNow.Date
                && provider.ProviderPairAssetSyncInfo.LastPairSync.Date == DateTime.UtcNow.Date
                && provider.CandlestickSyncInfos.Exists(candlestickSyncInfo =>
                    candlestickSyncInfo?.LastCandlestickSync.Date == DateTime.UtcNow.Date
                    && candlestickSyncInfo.Timeframe == timeframe
                    && timeframe == Timeframe.Daily))
            {
                return true;
            }

            DateTime currentDate = DateTime.UtcNow;
            DayOfWeek currentDayOfWeek = currentDate.DayOfWeek;

            int dayDifference = (currentDayOfWeek - DayOfWeek.Monday + 7) % 7;

            return provider.ProviderPairAssetSyncInfo.LastAssetSync.Date == DateTime.UtcNow.Date
            && provider.ProviderPairAssetSyncInfo.LastPairSync.Date == DateTime.UtcNow.Date
            && provider.CandlestickSyncInfos.Exists(candlestickSyncInfo =>
                    candlestickSyncInfo?.LastCandlestickSync.Date == DateTime.UtcNow.Date
                    && dayDifference > 0
                    && candlestickSyncInfo.Timeframe == timeframe
                    && timeframe == Timeframe.Weekly);
        }

        public static void UpdateProviderInfo(this ProviderSynchronization provider)
        {
            if (provider is null)
            {
                return;
            }
            provider.ProviderPairAssetSyncInfo.LastAssetSync = DateTime.UtcNow;
            provider.ProviderPairAssetSyncInfo.LastPairSync = DateTime.UtcNow;
        }

        public static ProviderCandlestickSyncInfo GetOrCreateProviderCandlestickSyncInfo(this ProviderSynchronization providerSynchronization, DataProvider provider, Timeframe timeframe)
        {
            var providerCandlestickSyncInfoProviderFound = providerSynchronization.CandlestickSyncInfos
                .Find(c => c.DataProvider == provider && c.Timeframe == timeframe);

            if (providerCandlestickSyncInfoProviderFound is null)
            {
                var newProviderCandlestickSyncInfo = new ProviderCandlestickSyncInfo
                {
                    DataProvider = providerSynchronization.ProviderPairAssetSyncInfo.DataProvider,
                    Timeframe = timeframe,
                    LastCandlestickSync = DateTime.UtcNow
                };
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
