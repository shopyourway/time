using System;

namespace OhioBox.Time
{
	public static class DateTimeExtensions
	{
		private static readonly TimeZoneInfo CstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

		public static DateTime ConvertFromUtcToCst(this DateTime target)
		{
			return TimeZoneInfo.ConvertTime(target, TimeZoneInfo.Utc, CstZone);
		}
	}
}
