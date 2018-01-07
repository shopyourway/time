namespace OhioBox.Time
{
	public class FiscalDateMedaData
	{
		public int Year { get; set; }
		public int Quarter { get; set; }
		public int Month { get; set; }
		public int Week { get; set; }

		public FiscalDateMedaData(int year, int quarter, int month, int week)
		{
			Year = year;
			Quarter = quarter;
			Month = month;
			Week = week;
		}
	}
}