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
        public void ParseCandlestickData_SetsDecimalProperty_Nullable()
        {
            var candlestick = new BinanceCandlestick();
            var cell = "12345.6789";
            var property = typeof(BinanceCandlestick).GetProperty(nameof(BinanceCandlestick.OpenPrice));

            CandlestickExtension.ParseCandlestickData(candlestick, cell, property);

            Assert.Equal(12345.6789m, candlestick.OpenPrice);
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

    }
}
