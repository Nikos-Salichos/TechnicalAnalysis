using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class DataProviderExtension
    {
        public static bool IsProviderAssetPairsSyncedToday(this ProviderSynchronization? provider)
        {
            if (provider is null)
            {
                return false;
            }

            var currentDate = DateTime.UtcNow.Date;

            return provider.ProviderPairAssetSyncInfo.LastAssetSync.Date == currentDate
                && provider.ProviderPairAssetSyncInfo.LastPairSync.Date == currentDate;
        }

        public static bool IsProviderCandlesticksSyncedToday(this ProviderSynchronization? provider, Timeframe timeframe)
        {
            if (provider is null)
            {
                return false;
            }

            if (provider.CandlestickSyncInfos.Exists(candlestickSyncInfo =>
                    candlestickSyncInfo?.LastCandlestickSync.Date == DateTime.UtcNow.Date
                    && candlestickSyncInfo.Timeframe == timeframe
                    && timeframe == Timeframe.Daily))
            {
                return true;
            }

            DateTime currentDate = DateTime.UtcNow;
            DayOfWeek currentDayOfWeek = currentDate.DayOfWeek;

            int daysSinceMonday = (currentDayOfWeek - DayOfWeek.Monday + 7) % 7;

            return provider.CandlestickSyncInfos.Exists(candlestickSyncInfo =>
                    candlestickSyncInfo?.LastCandlestickSync.Date == DateTime.UtcNow.Date
                    && daysSinceMonday > 0
                    && candlestickSyncInfo.Timeframe == timeframe
                    && timeframe == Timeframe.Weekly);
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
