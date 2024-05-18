using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.CommonModels.BusinessModels;

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
            var result = DataProviderExtension.IsProviderAssetPairsSyncedToday(provider);

            // Assert
            Assert.False(result);
        }

    }
}
