using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.CommonModels.BusinessModels;
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

        [Fact]
        public void FindNewCandlesticks_NoExistingCandles_NoChanges()
        {
            // Arrange
            var fixedDatetime = new DateTime(2024, 5, 14, 12, 0, 0);

            var pairs = new List<BinancePair>
            {
                new() { Pair = "BTCUSDT", BinanceCandlesticks =
                    [
                    new() { OpenTime = fixedDatetime, CloseTime = fixedDatetime, Period = "1h" },
                    new() { OpenTime = fixedDatetime, CloseTime = fixedDatetime, Period = "1h" }
                    ]
            },
                new() { Pair = "ETHUSDT", BinanceCandlesticks =
                    [
                    new() { OpenTime = fixedDatetime, CloseTime = fixedDatetime, Period = "1h" },
                    new() { OpenTime = fixedDatetime, CloseTime = fixedDatetime, Period = "1h" }
                    ]
                }
            };

            var pairsWithExistingCandles = new List<BinancePair>();

            // Act
            pairs.FindNewCandlesticks(pairsWithExistingCandles);

            // Assert
            Assert.Collection(pairs,
                pair =>
                {
                    Assert.Equivalent(2, pair.BinanceCandlesticks.Count);
                    Assert.Collection(pair.BinanceCandlesticks,
                        candlestick => Assert.Equivalent(fixedDatetime, candlestick.OpenTime),
                        candlestick => Assert.Equivalent(fixedDatetime, candlestick.OpenTime)
                    );
                },
                pair =>
                {
                    Assert.Equivalent(2, pair.BinanceCandlesticks.Count);
                    Assert.Collection(pair.BinanceCandlesticks,
                        candlestick => Assert.Equivalent(fixedDatetime, candlestick.OpenTime),
                        candlestick => Assert.Equivalent(fixedDatetime, candlestick.OpenTime)
                    );
                }
            );
        }

        [Fact]
        public void FindNewCandlesticks_WithExistingCandles_FindNewCandles()
        {
            // Arrange
            var fixedDatetime = new DateTime(2024, 5, 14, 12, 0, 0);
            var newFixedDatetime = new DateTime(2025, 5, 14, 12, 0, 0);

            var existingListOfCandlesticks = new List<BinancePair>{ new()
            {
                Pair = "BTCUSDT", BinanceCandlesticks =
                    [
                    new() { OpenTime = fixedDatetime, CloseTime = fixedDatetime, Period = "1h" },
                    ]
               },
            };

            var listOfNewCandlesticks = new List<BinancePair> { new()
            {
                Pair = "BTCUSDT", BinanceCandlesticks =
                    [
                    new() { OpenTime = newFixedDatetime, CloseTime = newFixedDatetime, Period = "1h" }
                    ]
                }
            };

            // Act
            existingListOfCandlesticks.FindNewCandlesticks(listOfNewCandlesticks);

            // Assert
            Assert.Collection(listOfNewCandlesticks,
                pair =>
                {
                    Assert.Single(pair.BinanceCandlesticks);
                    Assert.Equivalent(newFixedDatetime, pair.BinanceCandlesticks[0].OpenTime);
                }
            );
        }
    }
}
