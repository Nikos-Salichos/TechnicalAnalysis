using System.Globalization;

namespace TechnicalAnalysis.Application.Extensions
{
    public static class NumberExtension
    {
        public static long? ReduceDigitsToFitLongLength(this string? stringWithComma)
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
                return null;
            }
        }

        public static decimal? ReduceDigitsToFitDecimalLength(this string? stringWithComma)
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
                return Math.Round(decimalNumber, 6);
            }
            else
            {
                return null;
            }
        }

        public static long ToLong(this string? value)
        {
            if (long.TryParse(value, out long result))
            {
                return result;
            }

            throw new ArgumentException("Invalid long format", nameof(value));
        }
    }
}
