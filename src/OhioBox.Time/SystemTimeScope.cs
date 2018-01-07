using System;

namespace OhioBox.Time
{
	public class SystemTimeScope : IDisposable
	{
		private readonly Func<DateTime> _systemTime = SystemTime.Now;

		public SystemTimeScope(Func<DateTime> now)
		{
			SystemTime.Now = now;
		}

		public SystemTimeScope(DateTime now)
			: this(() => now)
		{
		}

		public void Dispose()
		{
			SystemTime.Now = _systemTime;
		}

		public static SystemTimeScope New(DateTime now)
		{
			return new SystemTimeScope(now);
		}

		public static SystemTimeScope New(Func<DateTime> now)
		{
			return new SystemTimeScope(now);
		}
	}
}