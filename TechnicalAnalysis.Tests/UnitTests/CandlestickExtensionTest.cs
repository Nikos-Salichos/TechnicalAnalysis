using TechnicalAnalysis.Application.Extensions;
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


    }
}
