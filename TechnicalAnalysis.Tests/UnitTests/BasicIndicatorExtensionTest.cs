using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Indicators.Basic;

namespace TechnicalAnalysis.Tests.UnitTests
{
    public class BasicIndicatorExtensionTest
    {
        [Fact]
        public void CalculateRsiExtreme_ShouldAssignHighestAndLowestRsiValues()
        {
            // Arrange
            var candlesticks = new List<CandlestickExtended>();
            var hardcodedRsiValues = new List<int>
            {
                10, 15, 20, 25, 30, 35, 40, 45, 50, 55,
                60, 65, 70, 75, 80, 85, 90, 95, 5, 3,
                25, 48, 67, 78, 89, 90, 12, 34, 56, 78,
                12, 23, 34, 45, 56, 67, 78, 89, 91, 92,
                20, 22, 24, 26, 28, 30, 32, 34, 36, 38,
                10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
                21, 23, 25, 27, 29, 31, 33, 35, 37, 39,
                5, 10, 15, 20, 25, 30, 35, 40, 45, 50,
                55, 60, 65, 70, 75, 80, 85, 90, 95, 99,
                3, 6, 9, 12, 15, 18, 21, 24, 27, 30
            };

            int count = 0;
            foreach (var rsiValue in hardcodedRsiValues)
            {
                var rsi = new Rsi(++count)
                {
                    Period = 10,
                    Overbought = 80,
                    Oversold = 20,
                    Value = rsiValue,
                    NumberOfRsiLowerThanPreviousRsis = 0,
                };

                var candlestickExtended = new CandlestickExtended
                {
                    PrimaryId = count
                };
                candlestickExtended.Rsis.Add(rsi);

                candlesticks.Add(candlestickExtended);
            }

            // Act
            BasicIndicatorExtension.CalculateRsiExtreme(candlesticks);

            // Assert
            const int lookbackPeriod = 30;
            for (int i = 0; i < candlesticks.Count; i++)
            {
                int start = Math.Max(0, i - lookbackPeriod + 1);
                var chunk = candlesticks.Skip(start).Take(lookbackPeriod).ToList();
                var rsiValues = chunk.Select(c => c.Rsis[0].Value).ToList();

                double? expectedHighestRsi = rsiValues.Max();
                double? expectedLowestRsi = rsiValues.Min();

                var dynamicRsi = candlesticks[i].DynamicRsis[^1]; // Assuming DynamicRsis is appended in CalculateRsiExtreme
                Assert.Equal(expectedHighestRsi.Value, dynamicRsi.Overbought);
                Assert.Equal(expectedLowestRsi.Value, dynamicRsi.Oversold);
            }
        }

        [Fact]
        public void CalculateRsiExtreme_NoLookbackPeriod_NoValuesInDynamicRsi()
        {
            // Arrange
            var candlesticks = new List<CandlestickExtended>();

            var candlestickExtended = new CandlestickExtended
            {
                PrimaryId = 1
            };

            candlesticks.Add(candlestickExtended);

            // Act
            BasicIndicatorExtension.CalculateRsiExtreme(candlesticks);

            // Assert

            foreach (var candlestick in candlesticks)
            {
                Assert.Empty(candlestick.DynamicRsis);
            }
        }

    }
}
