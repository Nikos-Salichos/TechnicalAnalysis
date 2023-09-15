using System.Globalization;

namespace TechnicalAnalysis.Domain.Extensions
{
    public static class NumberExtensions
    {
        public static long? ReduceDigitsToFitLongLength(this string stringWithComma)
        {
            if (string.IsNullOrWhiteSpace(stringWithComma))
            {
                return null;
            }

            // Remove any non-digit characters
            stringWithComma = new string(stringWithComma.Where(char.IsDigit).ToArray());

            // Trim the number string to the first 10 digits if it's too long
            if (stringWithComma.Length > 10)
            {
                stringWithComma = stringWithComma[..10];
            }

            // Try to parse the number string into a long
            if (long.TryParse(stringWithComma, out long parsedNumber))
            {
                return parsedNumber;
            }
            else
            {
                throw new ArgumentException("The provided string cannot be converted to a long.");
            }
        }

        public static decimal? ReduceDigitsToFitDecimalLength(this string stringWithComma)
        {
            if (string.IsNullOrWhiteSpace(stringWithComma))
            {
                return null;
            }

            string stringWithoutComma = stringWithComma.Replace(',', '.');
            if (stringWithoutComma.Length > 25)
            {
                stringWithoutComma = stringWithoutComma[..25];
            }

            if (decimal.TryParse(stringWithoutComma, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalNumber))
            {
                return decimalNumber;
            }
            else
            {
                throw new ArgumentException("The provided string cannot be converted to a decimal.");
            }
        }
    }
}
