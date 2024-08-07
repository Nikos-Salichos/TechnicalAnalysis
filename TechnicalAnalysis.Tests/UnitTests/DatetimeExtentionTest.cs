﻿using FluentAssertions;
using TechnicalAnalysis.Application.Extensions;

namespace TechnicalAnalysis.Tests.UnitTests
{
    [Trait("Category", "UnitTests-DatetimeExtension")]
    public class DatetimeExtentionTest
    {
        [Theory]
        [InlineData("2024-05-14 10:30", "2024-05-14 10:30", true)] // Same date and time
        [InlineData("2024-05-14 10:30", "2024-05-14 10:31", false)] // Different minute
        [InlineData("2024-05-14 10:30", "2024-05-14 11:30", false)] // Different hour
        [InlineData("2024-05-14 10:30", "2024-05-15 10:30", false)] // Different day
        [InlineData("2024-05-14 10:30", "2024-06-14 10:30", false)] // Different month
        [InlineData("2024-05-14 10:30", "2025-05-14 10:30", false)] // Different year
        public void EqualsYearMonthDayHourMinute_True(string datetimeStr1, string datetimeStr2, bool expectedResult)
        {
            // Arrange
            var datetime1 = DateTime.Parse(datetimeStr1);
            var datetime2 = DateTime.Parse(datetimeStr2);

            // Act
            var result = datetime1.EqualsYearMonthDayHourMinute(datetime2);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("2024-05-01", "2024-05-31", 1)] // Different day
        [InlineData("2024-05-01", "2024-06-01", 1)] // Different month
        [InlineData("2024-05-01", "2024-07-01", 1)] // Different year
        [InlineData("2014-05-01", "2025-05-31", 5)] // Different decade
        public void GetDailyDateRanges_ReturnsCorrectRanges(string startDateStr, string endDateStr, int countOfDateRanges)
        {
            // Arrange
            var startDate = DateTime.Parse(startDateStr);
            var endDate = DateTime.Parse(endDateStr);

            // Act
            var dateRanges = DatetimeExtension.GetDailyDateRanges(startDate, endDate);

            // Assert
            dateRanges.Count.Should().Be(countOfDateRanges);
        }

        [Theory]
        [InlineData("2024-05-01", "2024-05-02", 1)] // Different day
        [InlineData("2024-05-01", "2024-05-10", 1)] // Different month
        [InlineData("2024-05-01", "2024-06-01", 1)] // Different year
        [InlineData("2014-05-01", "2025-05-31", 98)] // Different decade
        public void GetHourlyDateRanges_ReturnsCorrectRanges(string startDateStr, string endDateStr, int countOfDateRanges)
        {
            // Arrange
            var startDate = DateTime.Parse(startDateStr);
            var endDate = DateTime.Parse(endDateStr);

            // Act
            var dateRanges = DatetimeExtension.GetHourlyDateRanges(startDate, endDate);

            // Assert
            dateRanges.Count.Should().Be(countOfDateRanges);
        }

        [Theory]
        [InlineData("2024-05-01", "2024-05-31", 1)] // Same month, multiple weeks
        [InlineData("2024-05-01", "2024-06-01", 1)] // Different month
        [InlineData("2024-05-01", "2024-07-01", 1)] // Different year
        [InlineData("2004-05-01", "2025-05-31", 2)] // Different decades
        [InlineData("2024-05-01", "2024-05-07", 1)] // Same week
        public void GetWeeklyDateRanges_ReturnsCorrectRanges(string startDateStr, string endDateStr, int countOfDateRanges)
        {
            // Arrange
            var startDate = DateTime.Parse(startDateStr);
            var endDate = DateTime.Parse(endDateStr);

            // Act
            var dateRanges = DatetimeExtension.GetWeeklyDateRanges(startDate, endDate);

            // Assert
            dateRanges.Count.Should().Be(countOfDateRanges);
        }
    }
}
