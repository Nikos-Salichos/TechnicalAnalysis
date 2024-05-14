using FluentAssertions;
using TechnicalAnalysis.Application.Extensions;

namespace TechnicalAnalysis.Tests.UnitTests
{
    [Trait("Category", "UnitTests-DatetimeExtension")]
    public class DatetimeExtentionTest
    {
        [Theory]
        [InlineData("2024-05-14 10:30", "2024-05-14 10:30", true)] // Same date and time

        public void EqualsYearMonthDayHourMinute_True(string datetimeStr1, string datetimeStr2, bool expectedResult)
        {
            // Arrange
            var datetime1 = DateTime.Parse(datetimeStr1);
            var datetime2 = DateTime.Parse(datetimeStr2);

            // Act
            var result = DatetimeExtension.EqualsYearMonthDayHourMinute(datetime1, datetime2);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
