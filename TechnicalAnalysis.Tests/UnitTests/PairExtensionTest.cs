using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Tests.UnitTests
{
    public class PairExtensionTests
    {
        [Theory]
        [InlineData("BTC-USDT", true)]
        [InlineData("BTC-USDC", false)]
        [InlineData("ETH-USDC", true)]
        [InlineData("ADA-DAI", true)]
        [InlineData("BTC-BUSD", false)]
        [InlineData("DOT-BUSD", true)]
        [InlineData("BTC-ETH", false)]
        public void GetDollarPairs_ReturnsUniqueDollarPairs(string pair, bool expectedResult)
        {
            // Arrange
            var binanceAssets = new List<Asset> {
            new() { PrimaryId = 1, Symbol = "USDT" } ,
            new() { PrimaryId = 2, Symbol = "USDC" },
            new() { PrimaryId = 3, Symbol = "DAI" },
            new() { PrimaryId = 4, Symbol = "BUSD" },
            new() { PrimaryId = 100, Symbol = "ETH" },
            new() { PrimaryId = 30, Symbol = "ADA" },
            };

            var binancePair = new List<PairExtended> {
            new() { BaseAssetId = 10, QuoteAssetId = 1, Symbol = "BTC-USDT"} ,
            new() { BaseAssetId = 10, QuoteAssetId = 2, Symbol = "BTC-USDC"} ,
            new() { BaseAssetId = 20, QuoteAssetId = 2, Symbol = "ETH-USDC"} ,
            new() { BaseAssetId = 30, QuoteAssetId = 3, Symbol = "ADA-DAI"} ,
            new() { BaseAssetId = 10, QuoteAssetId = 4, Symbol = "BTC-BUSD"} ,
            new() { BaseAssetId = 50, QuoteAssetId = 4, Symbol = "DOT-BUSD"} ,
            new() { BaseAssetId = 10, QuoteAssetId = 100, Symbol = "BTC-ETH"} ,
            new() { BaseAssetId = 10, QuoteAssetId = 30, Symbol = "BTC-ADA"} ,
            };

            // Act
            var dollarPairs = PairExtension.GetUniqueDollarPairs(binanceAssets, binancePair);

            // Assert
            Assert.Equal(expectedResult, dollarPairs.Exists(p => p.Symbol == pair));
        }
    }

}
