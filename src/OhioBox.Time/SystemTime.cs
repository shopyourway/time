using System;

namespace OhioBox.Time
{
	public static class SystemTime
	{
		public static Func<DateTime> Now = () => DateTime.UtcNow;

		public static Func<DateTime> Today = () => Now().Date;

		public static Func<DateTime> CstNow = () => Now().ConvertFromUtcToCst();

		public static Func<DateTime> CstToday = () => CstNow().Date;
	}
}
