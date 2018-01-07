using System;
using NUnit.Framework;

namespace OhioBox.Time.Tests
{
	[TestFixture]
	public class DateTimeExtensionsTests
	{
		// The following tests relay on the fact that on Mar 9, 2014 at 2:00 CST, the clocks moved 1 hour forward to 3:00 CST

		[Test]
		public void ConvertFromUtcToCst_BeforeCstDaylightSavingTimeChange_ReturnCstDate()
		{
			var utcTime = new DateTime(2014, 3, 9, 7, 30, 49);
			var expected = new DateTime(2014, 3, 9, 1, 30, 49);

			var cstTime = utcTime.ConvertFromUtcToCst();

			Assert.That(cstTime, Is.EqualTo(expected));
		}

		[Test]
		public void ConvertFromUtcToCst_AfterCstDaylightSavingTimeChange_ReturnCstDate()
		{
			var utcTime = new DateTime(2014, 3, 9, 8, 30, 49);
			var expected = new DateTime(2014, 3, 9, 3, 30, 49);

			var cstTime = utcTime.ConvertFromUtcToCst();

			Assert.That(cstTime, Is.EqualTo(expected));
		}
	}
}