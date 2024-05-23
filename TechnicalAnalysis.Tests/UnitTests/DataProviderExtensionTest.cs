using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Tests.UnitTests
{
    public class DataProviderExtensionTest
    {
        [Fact]
        public void IsProviderAssetPairsSyncedToday_ProviderIsNull_ReturnsFalse()
        {
            // Arrange
            ProviderSynchronization provider = null;

            // Act
            var result = provider.IsProviderAssetPairsSyncedToday();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsProviderAssetPairsSyncedTodayProvider_ReturnsTrue()
        {
            // Arrange
            ProviderSynchronization provider = new();
            provider.ProviderPairAssetSyncInfo.UpdateProviderInfo();

            // Act
            var result = provider.IsProviderAssetPairsSyncedToday();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsProviderCandlesticksSyncedToday_WithNullProvider_ReturnsFalse()
        {
            // Arrange
            ProviderSynchronization provider = null;

            // Act
            var result = provider.IsProviderCandlesticksSyncedToday(Timeframe.Daily);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsProviderCandlesticksSyncedToday_DailySyncToday_ReturnsTrue()
        {
            // Arrange
            var provider = new ProviderSynchronization
            {
                CandlestickSyncInfos = new List<ProviderCandlestickSyncInfo>
            {
                new() {
                    LastCandlestickSync = DateTime.UtcNow.Date,
                    Timeframe = Timeframe.Daily
                }
            }
            };

            // Act
            var result = provider.IsProviderCandlesticksSyncedToday(Timeframe.Daily);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsProviderCandlesticksSyncedToday_DailySyncNotToday_ReturnsFalse()
        {
            // Arrange
            var provider = new ProviderSynchronization
            {
                CandlestickSyncInfos = new List<ProviderCandlestickSyncInfo>
            {
                new() {
                    LastCandlestickSync = DateTime.UtcNow.Date.AddDays(-1),
                    Timeframe = Timeframe.Daily
                }
            }
            };

            // Act
            var result = provider.IsProviderCandlesticksSyncedToday(Timeframe.Daily);

            // Assert
            Assert.False(result);
        }
    }
}
