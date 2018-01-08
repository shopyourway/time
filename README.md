# OhioBox.Time

`OhioBox.Time` is a wrapper for DateTime, allowing an application to use the same, consistent DateTime object and writing proper tests for services which needs date or time.<br/>
`OhioBox.Time` provides a single entry point for using date related operations called `SystemTime.Now` represents an instance of UTC `DateTime` and can be controlled and mocked to for testing purposes.

## Getting started

### Installation
[![NuGet](https://img.shields.io/nuget/v/OhioBox.Time.svg?style=flat-square)](https://www.nuget.org/packages/OhioBox.Time/)

### How to use
Getting the current date and time is done using `Now` method:
```cs
using System;
using OhioBox.Time;

class Progam
{
	public static void Main()
	{
		var currentDateTime = SystemTime.Now(); // Returns a DateTime instance

		Console.WriteLine(currentDateTime.ToString("yyyy-MM-dd hh:mm"));
	}
}
```
The output here will be UTC date and time (i.e. `DateTime.UtcNow`).

### Testing
One of the more challanging problems in testing, is how to test usage of `DateTime`.

Assuming we have the following method:
```cs
public class RecentOrdersFetcher
{
	IList<Order> GetOrdersFromLastMonth()
	{
		var lastMonth = DateTime.Now.AddMonths(-1);
		...
	}
}
```
Testing this kind of service requires controling `DateTime.Now` functionality. Since most mocking frameworks don't allow overriding static methods & properties, most applications simply extract the usage of `DateTime.Now` to dependency or to another service.

Here's where `SystemTime` shows its strength. Converting the service to use `SystemTime` will result in:
```cs
using OhioBox.Time;

public class RecentOrdersFetcher
{
	IList<Order> GetOrdersFromLastMonth()
	{
		var lastMonth = SystemTime.Now().AddMonths(-1);
		...
	}
}
```
And in order to test it, lets use `SystemTimeScope`:
```cs
[Test]
public void GetOrdersFromLastMonth_Called_ReturnsAllOrdersFromLastMonth()
{
	var today = new DateTime(2018, 1, 7);

	using (new SystemTimeScope(() => today))
	{
		// Arrange test here

		var result = _target.GetOrdersFromLastMonth();

		// Assert here
	}
}
```
Inside the scope of `SystemTimeScope`, a call to `SystemTime.Now()` will result with `2018-1-7` DateTime instance. <br/>
Once the scope is completed, `SystemTime.Now()` resets to its default (`UtcNow`).

### DateTime Extensions

#### UTC to CST
```cs
var date = new DateTime(2018, 1, 7, 10, 34, 22, DateTimeKind.Utc);
var cstDate = SystemTime.Now().ConvertFromUtcToCst(); // Will return 2018-1-7 4:34:22
```
#### Fiscal Date MetaData
Calculates the current fiscal date based on 4-5-4 fiscal calendar method.
```cs
var currentFiscalMetaData = SystemTime.CstNow().GetFiscalMetaData();
Console.WriteLine($"The current fiscal medata data are: year: {currentFiscalMetaData.Year}, quarter: {currentFiscalMetaData.Quarter}, month {currentFiscalMetaData.Month} and week: {currentFiscalMetaData.Week}");
			
## Development

### How to contribute
We encorage contribution via pull requests on any feature you see fit.

When submitting a pull request make sure to do the following:
* Check that new and updated code follows OhioBox existing code formatting and naming standard
* Run all unit and integration tests to ensure no existing functionality has been affected
* Write unit or integration tests to test your changes. All features and fixed bugs must have tests to verify they work
Read [GitHub Help](https://help.github.com/articles/about-pull-requests/) for more details about creating pull requests