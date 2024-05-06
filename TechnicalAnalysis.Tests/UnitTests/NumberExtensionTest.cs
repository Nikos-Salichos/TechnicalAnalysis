using FluentAssertions;
using System.Globalization;
using TechnicalAnalysis.Domain.Extensions;

namespace TechnicalAnalysis.Tests.UnitTests
{
    [Trait("Category", "UnitTests-NumberExtension")]
    public class NumberExtensionTest
    {
        [Fact]
        public void ReduceDigitsToFitLongLength_ReturnsSuccess()
        {
            const string numberAsString = "12345678901234567890";
            var longNumberWithTenDigits = numberAsString.ReduceDigitsToFitLongLength();

            longNumberWithTenDigits.Should().NotBeNull();
            longNumberWithTenDigits.Value.ToString().Length.Should().Be(10);
        }

        [Fact]
        public void ReduceDigitsToFitLongLength_WhenStringHasCharacterInTheEnd_ReturnsSuccess()
        {
            const string numberAsString = "12345678901234567890nikos";
            var longNumberWithTenDigits = numberAsString.ReduceDigitsToFitLongLength();

            longNumberWithTenDigits.Should().NotBeNull();
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Length.Should().Be(10);
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Should().NotContain("nikos");
        }

        [Fact]
        public void ReduceDigitsToFitLongLength_WhenStringBeginWithCharacter_ReturnsSuccess()
        {
            const string numberAsString = "nikos12345678901234567890";
            var longNumberWithTenDigits = numberAsString.ReduceDigitsToFitLongLength();

            longNumberWithTenDigits.Should().NotBeNull();
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Length.Should().Be(10);
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Should().NotContain("nikos");
        }

        [Fact]
        public void ReduceDigitsToFitLongLength_WhenStringContainsCharacterInTheMid_ReturnsSuccess()
        {
            const string numberAsString = "1234567890nikos1234567890";
            var longNumberWithTenDigits = numberAsString.ReduceDigitsToFitLongLength();

            longNumberWithTenDigits.Should().NotBeNull();
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Length.Should().Be(10);
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Should().NotContain("nikos");
        }

        [Fact]
        public void ReduceDigitsToFitDecimalLength_ReturnsSuccess()
        {
            const string numberAsString = "123456789012345678901234567890,12345678901234567890";
            var longNumberWithTenDigits = numberAsString.ReduceDigitsToFitDecimalLength();

            longNumberWithTenDigits.Should().NotBeNull();
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Length.Should().Be(25);
        }

        [Fact]
        public void ReduceDigitsToFitDecimalLength_WhenCommaIsInFirst25Digits_ReturnsWithComma()
        {
            const string numberAsString = "1234567890,1234567890123456789012345678901234567890";
            var longNumberWithTenDigits = numberAsString.ReduceDigitsToFitDecimalLength();

            longNumberWithTenDigits.Should().NotBeNull();
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Length.Should().Be(25);
            longNumberWithTenDigits.Value.ToString(CultureInfo.InvariantCulture).Should().Contain(",");
        }
    }
}
