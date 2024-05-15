namespace TechnicalAnalysis.Application.Extensions
{
    public static class DatetimeExtension
    {
        public static bool EqualsYearMonthDayHourMinute(this DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year &&
                   dt1.Month == dt2.Month &&
                   dt1.Day == dt2.Day &&
                   dt1.Hour == dt2.Hour &&
                   dt1.Minute == dt2.Minute;
        }

        public static List<(DateTime, DateTime)> GetDailyDateRanges(DateTime startDate, DateTime endDate)
        {
            const int maximumDaysPerRange = 1000;

            var dateRanges = new List<(DateTime, DateTime)>();
            var daysRemaining = (int)(endDate - startDate).TotalDays;
            var batchStart = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            while (daysRemaining > maximumDaysPerRange)
            {
                var daysInBatch = Math.Min(maximumDaysPerRange, daysRemaining);
                var batchEnd = batchStart.AddDays(daysInBatch).Date.Add(new TimeSpan(23, 59, 59));
                dateRanges.Add((batchStart, batchEnd));
                daysRemaining -= daysInBatch;
                batchStart = batchEnd.AddDays(1).Date;
            }

            if (daysRemaining > 0 || (daysRemaining == 0 && endDate - startDate == new TimeSpan(23, 59, 59)))
            {
                var lastBatchEnd = endDate.Date.Add(new TimeSpan(23, 59, 59));
                daysRemaining = (int)(endDate - batchStart).TotalDays;
                var lastBatchStart = lastBatchEnd.AddDays(-daysRemaining).Date;
                dateRanges.Add((lastBatchStart, lastBatchEnd));
            }

            return dateRanges;
        }

        public static List<(DateTime, DateTime)> GetWeeklyDateRanges(DateTime startDate, DateTime endDate)
        {
            const int maximumDaysPerRange = 1000;

            var dateRanges = new List<(DateTime, DateTime)>();
            var weeksRemaining = (int)Math.Ceiling((endDate - startDate).TotalDays / 7.0);
            var batchStart = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            while (weeksRemaining > maximumDaysPerRange)
            {
                var weeksInBatch = Math.Min(maximumDaysPerRange, weeksRemaining);
                var batchEnd = batchStart.AddDays(weeksInBatch * 7).Date.Add(new TimeSpan(23, 59, 59));
                dateRanges.Add((batchStart, batchEnd));
                weeksRemaining -= weeksInBatch;
                batchStart = batchEnd.AddDays(1).Date;
            }

            if (weeksRemaining > 0)
            {
                var maxEndDate = DateTime.UtcNow.AddDays(-1).Date.Add(new TimeSpan(23, 59, 59));
                var lastBatchEnd = endDate < maxEndDate ? endDate : maxEndDate;

                DateTime lastBatchStart;
                DateTime initialStart = lastBatchEnd.AddDays(-((weeksRemaining * 7) + 1));
                if (initialStart.DayOfWeek == DayOfWeek.Monday)
                {
                    lastBatchStart = initialStart.Date;
                }
                else
                {
                    DateTime nextMonday = initialStart.Date.AddDays((DayOfWeek.Monday + 7 - initialStart.DayOfWeek) % 7);
                    lastBatchStart = nextMonday.Date;
                }

                if (lastBatchEnd.DayOfWeek == DayOfWeek.Sunday)
                {
                    lastBatchStart = lastBatchEnd.AddDays(-((weeksRemaining * 7) + 1));
                    dateRanges.Add((lastBatchStart, lastBatchEnd));
                }
                else
                {
                    var previousSunday = lastBatchEnd.AddDays(-(int)lastBatchEnd.DayOfWeek);
                    lastBatchEnd = new DateTime(previousSunday.Year, previousSunday.Month, previousSunday.Day, 23, 59, 59);
                    dateRanges.Add((lastBatchStart, lastBatchEnd));
                }
            }

            return dateRanges;
        }

        public static List<(DateTime, DateTime)> GetHourlyDateRanges(DateTime startDate, DateTime endDate)
        {
            var dateRanges = new List<(DateTime start, DateTime end)>();
            DateTime currentStart = startDate;
            DateTime currentEnd = currentStart.AddHours(1000).AddSeconds(-1); // Set end time to last second of last hour
            while (currentEnd < endDate)
            {
                dateRanges.Add((currentStart, currentEnd));
                currentStart = currentEnd.AddSeconds(1); // Set start time to first second of next hour
                currentEnd = currentStart.AddHours(1000).AddSeconds(-1); // Set end time to last second of last hour
            }
            dateRanges.Add((currentStart, endDate));
            return dateRanges;
        }
    }
}
