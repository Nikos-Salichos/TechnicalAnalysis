using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;

namespace TechnicalAnalysis.Tests.UnitTests
{
    public class CandlestickExtensionTest
    {
        [Fact]
        public void ParseCandlestickData_SetsDecimalProperty_Correctly()
        {
            var candlestick = new BinanceCandlestick();
            var cell = "12345.6789";
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.OpenPrice));

            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            Assert.Equal(12345.6789m, candlestick.OpenPrice);
        }

        [Fact]
        public void ParseCandlestickData_SetsDecimalProperty_WithNullValue()
        {
            // Arrange
            var candlestick = new BinanceCandlestick();
            string? cell = null;
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.OpenPrice));

            // Act
            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            // Assert
            Assert.Null(candlestick.OpenPrice);
        }

        [Fact]
        public void ParseCandlestickData_SetsDecimalProperty_WithEmptyString()
        {
            // Arrange
            var candlestick = new BinanceCandlestick();
            var cell = string.Empty;
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.OpenPrice));

            // Act
            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            // Assert
            Assert.Null(candlestick.OpenPrice);
        }

        [Fact]
        public void ParseCandlestickData_SetsDateTimeProperty_CorrectlyFromUnixTime()
        {
            var candlestick = new BinanceCandlestick();
            var cell = "1609459200000"; // 2021-01-01T00:00:00.000Z
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.OpenTime));

            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            Assert.Equal(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc), candlestick.OpenTime);
        }

        [Fact]
        public void ParseCandlestickData_SetsDateTimeProperty_CorrectlyFromString()
        {
            var candlestick = new BinanceCandlestick();
            var cell = "2021-01-01T00:00:00";
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.OpenTime));

            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            Assert.Equal(new DateTime(2021, 1, 1, 0, 0, 0), candlestick.OpenTime);
        }

        [Fact]
        public void ParseCandlestickData_SetsLongProperty_Correctly()
        {
            var candlestick = new BinanceCandlestick();
            var cell = "100000";
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.Volume));

            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            Assert.Equal(100000L, candlestick.Volume);
        }

        [Fact]
        public void ParseCandlestickData_DoesNotSetInvalidDecimalProperty()
        {
            var candlestick = new BinanceCandlestick();
            var cell = "invalid";
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.OpenPrice));

            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            Assert.Null(candlestick.OpenPrice);
        }

        [Fact]
        public void ParseCandlestickData_DoesNotSetInvalidDateTimeProperty()
        {
            var candlestick = new BinanceCandlestick();
            var cell = "invalid";
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.OpenTime));

            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            Assert.Equal(default, candlestick.OpenTime);
        }

        [Fact]
        public void ParseCandlestickData_DoesNotSetInvalidLongProperty()
        {
            var candlestick = new BinanceCandlestick();
            var cell = "invalid";
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.Volume));

            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            Assert.Null(candlestick.Volume);
        }

        [Theory]
        [InlineData("1d", Timeframe.Daily)]
        [InlineData("1w", Timeframe.Weekly)]
        [InlineData("1h", Timeframe.OneHour)]
        [InlineData("1D", Timeframe.Daily)]
        [InlineData("1W", Timeframe.Weekly)]
        [InlineData("1H", Timeframe.OneHour)]
        public void ToTimeFrame_ReturnsCorrectTimeframe_ForValidPeriods(string period, Timeframe expectedTimeframe)
        {
            var result = period.ToTimeFrame();
            Assert.Equal(expectedTimeframe, result);
        }

        [Theory]
        [InlineData("2d")]
        [InlineData("1m")]
        [InlineData("daily")]
        public void ToTimeFrame_ThrowsArgumentException_ForInvalidPeriods(string period)
        {
            var exception = Assert.Throws<ArgumentException>(() => period.ToTimeFrame());
            Assert.Equal($"Invalid period: {period}", exception.Message);
        }

        [Theory]
        [InlineData("EXTREMEFEAR", ValueClassificationType.ExtremeFear)]
        [InlineData("FEAR", ValueClassificationType.Fear)]
        [InlineData("NEUTRAL", ValueClassificationType.Neutral)]
        [InlineData("GREED", ValueClassificationType.Greed)]
        [InlineData("EXTREMEGREED", ValueClassificationType.ExtremeGreed)]
        [InlineData("extremefear", ValueClassificationType.ExtremeFear)]
        [InlineData("fear", ValueClassificationType.Fear)]
        [InlineData("neutral", ValueClassificationType.Neutral)]
        [InlineData("greed", ValueClassificationType.Greed)]
        [InlineData("extremegreed", ValueClassificationType.ExtremeGreed)]
        [InlineData(" EXTREME FEAR ", ValueClassificationType.ExtremeFear)]
        [InlineData(" FEAR ", ValueClassificationType.Fear)]
        [InlineData(" NEUTRAL ", ValueClassificationType.Neutral)]
        [InlineData(" GREED ", ValueClassificationType.Greed)]
        [InlineData(" EXTREME GREED ", ValueClassificationType.ExtremeGreed)]
        public void ToValueClassificationType_ReturnsCorrectValueClassificationType_ForValidInputs(string valueClassification,
            ValueClassificationType expectedType)
        {
            var result = valueClassification.ToValueClassificationType();
            Assert.Equal(expectedType, result);
        }

        [Theory]
        [InlineData("EXTREMEHAPPINESS")]
        [InlineData("SADNESS")]
        [InlineData("ANGRY")]
        [InlineData("JOY")]
        [InlineData("")]
        [InlineData("  ")]
        public void ToValueClassificationType_ThrowsArgumentException_ForInvalidInputs(string valueClassification)
        {
            var exception = Assert.Throws<ArgumentException>(() => valueClassification.ToValueClassificationType());
            Assert.Equal($"Invalid valueClassification: {new string(value: valueClassification
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray()).ToUpperInvariant()}", exception.Message);
        }

        [Fact]
        public void FillMissingDatesInDays_AddsMissingDates_Correctly()
        {
            var pairs = new List<BinancePair>
    {
        new() {
            BinanceCandlesticks =
            [
                new() { OpenTime = new(2023, 1, 1), CloseTime = new(2023, 1, 1, 23, 59, 59) },
                new() { OpenTime = new(2023, 1, 4), CloseTime = new(2023, 1, 4, 23, 59, 59) }
            ]
        }
    };

            pairs.FillMissingDatesInDays();

            var expectedDates = new List<DateTime>
    {
        new(2023, 1, 1),
        new(2023, 1, 2),
        new(2023, 1, 3),
        new(2023, 1, 4)
    };

            var actualDates = pairs[0].BinanceCandlesticks.OrderBy(c => c.OpenTime).Select(c => c.OpenTime.Date).ToList();

            Assert.Equal(expectedDates, actualDates);
        }

        [Fact]
        public void FillMissingDates_NoMissingDates()
        {
            var pairs = new List<BinancePair>
    {
        new() {
            BinanceCandlesticks =
            [
                new BinanceCandlestick { OpenTime = new DateTime(2023, 1, 1), CloseTime = new DateTime(2023, 1, 1, 23, 59, 59) },
                new BinanceCandlestick { OpenTime = new DateTime(2023, 1, 2), CloseTime = new DateTime(2023, 1, 2, 23, 59, 59) }
            ]
        }
    };

            pairs.FillMissingDatesInDays();

            var actualDates = pairs[0].BinanceCandlesticks.OrderBy(c => c.OpenTime).Select(c => c.OpenTime).ToList();
            var expectedDates = new List<DateTime>
    {
        new(2023, 1, 1),
        new(2023, 1, 2)
    };

            Assert.Equal(expectedDates, actualDates);
        }

        [Fact]
        public void FillMissingDatesInDays_MultiplePairs_AddsMissingDates_Correctly()
        {
            var pairs = new List<BinancePair>
    {
        new() {
            BinanceCandlesticks =
            [
                new() { OpenTime = new DateTime(2023, 1, 1), CloseTime = new DateTime(2023, 1, 1, 23, 59, 59) },
                new() { OpenTime = new DateTime(2023, 1, 3), CloseTime = new DateTime(2023, 1, 3, 23, 59, 59) }
            ]
        },
        new() {
            BinanceCandlesticks =
            [
                new() { OpenTime = new(2023, 1, 5), CloseTime = new(2023, 1, 5, 23, 59, 59) },
                new() { OpenTime = new(2023, 1, 7), CloseTime = new(2023, 1, 7, 23, 59, 59) }
            ]
        }
    };

            pairs.FillMissingDatesInDays();

            var expectedOpenTimesPair1 = new List<DateTime>
    {
        new(2023, 1, 1),
        new(2023, 1, 2),
        new(2023, 1, 3)
    };

            var expectedOpenTimesPair2 = new List<DateTime>
    {
        new(2023, 1, 5),
        new(2023, 1, 6),
        new(2023, 1, 7)
    };

            var actualOpenTimesPair1 = pairs[0].BinanceCandlesticks.OrderBy(c => c.OpenTime).Select(c => c.OpenTime.Date).ToList();
            var actualOpenTimesPair2 = pairs[1].BinanceCandlesticks.OrderBy(c => c.OpenTime).Select(c => c.OpenTime.Date).ToList();

            Assert.Equal(expectedOpenTimesPair1, actualOpenTimesPair1);
            Assert.Equal(expectedOpenTimesPair2, actualOpenTimesPair2);
        }

        [Fact]
        public void FillMissingDates_NoCandles()
        {
            var pairs = new List<BinancePair>
    {
        new() { BinanceCandlesticks = [] }
    };

            pairs.FillMissingDatesInDays();

            Assert.Empty(pairs[0].BinanceCandlesticks);
        }
    }
}
