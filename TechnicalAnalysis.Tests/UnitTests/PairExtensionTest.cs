using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;

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
            var binanceAssets = new List<BinanceAsset> {
            new BinanceAsset { Id = 1, Asset = "USDT" } ,
            new BinanceAsset { Id = 2, Asset = "USDC" },
            new BinanceAsset { Id = 3, Asset = "DAI" },
            new BinanceAsset { Id = 4, Asset = "BUSD" },
            new BinanceAsset { Id = 100, Asset = "ETH" },
            new BinanceAsset { Id = 30, Asset = "ADA" },
            };

            var binancePair = new List<BinancePair> {
            new BinancePair { BaseAssetId = 10, QuoteAssetId = 1, Pair = "BTC-USDT"} ,
            new BinancePair { BaseAssetId = 10, QuoteAssetId = 2, Pair = "BTC-USDC"} ,
            new BinancePair { BaseAssetId = 20, QuoteAssetId = 2, Pair = "ETH-USDC"} ,
            new BinancePair { BaseAssetId = 30, QuoteAssetId = 3, Pair = "ADA-DAI"} ,
            new BinancePair { BaseAssetId = 10, QuoteAssetId = 4, Pair = "BTC-BUSD"} ,
            new BinancePair { BaseAssetId = 50, QuoteAssetId = 4, Pair = "DOT-BUSD"} ,
            new BinancePair { BaseAssetId = 10, QuoteAssetId = 100, Pair = "BTC-ETH"} ,
            new BinancePair { BaseAssetId = 10, QuoteAssetId = 30, Pair = "BTC-ADA"} ,
            };

            // Act
            var dollarPairs = PairExtension.GetUniqueDollarPairs(binanceAssets, binancePair);

            // Assert
            Assert.Equal(expectedResult, dollarPairs.Any(p => p.Pair == pair));
        }
    }

}
