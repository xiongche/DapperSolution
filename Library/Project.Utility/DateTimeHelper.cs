using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System
{
    public sealed class DateTimeHelper
    {
        public static IList<Week> GetWeeks(DateTime from, DateTime to, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var cal = dfi.Calendar;

            var start = from;
            var weeks = new List<Week>();
            while (start.Date < to.Date)
            {
                var week = new Week() { StartDate = start };

                var endOfWeek = GetSundayOfWeek(start);
                var end = to.Date > endOfWeek ? endOfWeek : to;

                week.Year = GetWeekYear(start, rule, firstDayOfWeek);
                week.EndDate = end;
                week.WeekOfYear = cal.GetWeekOfYear(start, rule, firstDayOfWeek);
                weeks.Add(week);

                start = end.AddDays(1);
            }

            return weeks;
        }

        public static IList<Week> GetWeeks(int year, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
        {
            var from = new DateTime(year, 1, 1);
            var to = new DateTime(year, 12, 31);
            var weeks = GetWeeks(from, to, rule, firstDayOfWeek);

            return weeks.Where(w => w.Year == year).ToList();
        }

        public static DateTime GetMondayOfWeek(DateTime value)
        {
            int days = 1 - (int)value.DayOfWeek;

            return value.AddDays(days).Date;
        }

        public static DateTime GetSundayOfWeek(DateTime value)
        {
            if (value.DayOfWeek == DayOfWeek.Sunday)
            {
                return value;
            }

            int days = 7 - (int)value.DayOfWeek;
            return value.AddDays(days).Date.AddDays(1).AddSeconds(-1);
        }

        public static DateTime GetFirstDateOfMonth(DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1).Date;
        }

        public static DateTime GetLastDateOfMonth(DateTime value)
        {
            DateTime firstDayOfTheMonth = new DateTime(value.Year, value.Month, 1).Date;

            return firstDayOfTheMonth.AddMonths(1).AddDays(-1).Date;
        }

        public static DateTime Now()
        {
            return DateTime.Now;
        }

        public static DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }

        public static string GetTimestamp()
        {
            return Now().ToString("yyyyMMddHHmmssfff");
        }

        private static int GetWeekYear(DateTime value, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
        {
            if (rule == CalendarWeekRule.FirstDay || rule == CalendarWeekRule.FirstFullWeek)
            {
                return value.AddDays((int)firstDayOfWeek - (int)value.DayOfWeek).Year;
            }

            if ((int)value.DayOfWeek <= (int)DayOfWeek.Thursday && (int)value.DayOfWeek > 0)
            {
                return value.Year;
            }

            return value.AddDays(-3).Year;
        }
    }

    public class Week
    {
        public int Year
        { get; set; }

        public int WeekOfYear
        { get; set; }

        public DateTime StartDate
        { get; set; }

        public DateTime EndDate
        { get; set; }
    }
}