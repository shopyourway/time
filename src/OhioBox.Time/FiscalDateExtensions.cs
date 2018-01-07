using System;

namespace OhioBox.Time
{
	public static class FiscalDateExtensions
	{
		private const int QuarterDays = 91;
		private const int MonhtsInQuarter = 3;
		private const int DaysInFirstMonth = 28;
		private const int DaysInSecondMonth = 63;
		private const int LastQuarter = 4;
		private const int WeeksInYear = 52;
		private const int WeeksInMonth = 12;

		public static FiscalDateMedaData GetFiscalMetaData(this DateTime date)
		{
			var firstDateOfFiscalYear = GetFirstDayOfFiscalYear(new DateTime(date.Year, 2, 1));
			if (firstDateOfFiscalYear > date)
				firstDateOfFiscalYear = GetFirstDayOfFiscalYear(new DateTime(date.Year - 1, 2, 1));

			var year = GetYear(firstDateOfFiscalYear);
			var days = (date - firstDateOfFiscalYear).Days;
			var week = GetWeek(days);
			var quarter = GetQuarter(week, days);
			var month = GetMonth(quarter, week, days);

			return new FiscalDateMedaData(year, quarter, month, week);
		}

		public static int GetYear(DateTime date)
		{
			var firstDateOfFiscalYear = GetFirstDayOfFiscalYear(date);
			if (date < firstDateOfFiscalYear)
				return date.Year - 1;

			return date.Year;
		}

		private static int GetMonth(int quarter, int week, int days)
		{
			if (week > WeeksInYear)
				return WeeksInMonth;

			var baseMonth = (quarter - 1) * MonhtsInQuarter + 1;
			var daysInQuarter = days % QuarterDays;
			
			if (daysInQuarter < DaysInFirstMonth)
				return baseMonth;

			if (daysInQuarter < DaysInSecondMonth)
				return baseMonth + 1;

			return  baseMonth + 2;
		}

		private static int GetQuarter(int week, int days)
		{
			if (week > WeeksInYear)
				return LastQuarter;

			return days / QuarterDays + 1;
		}

		private static int GetWeek(int days)
		{
			return days / 7 + 1;
		}

		private static DateTime GetFirstDayOfFiscalYear(DateTime date)
		{
			if (date.DayOfWeek < DayOfWeek.Thursday)
				return date.AddDays(-1 * (date.DayOfWeek - DayOfWeek.Sunday));

			return date.AddDays(7 - (int)date.DayOfWeek);
		}
	}
}