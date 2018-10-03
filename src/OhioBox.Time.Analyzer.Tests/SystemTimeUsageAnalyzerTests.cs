using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace OhioBox.Time.Analyzer.Tests
{
	[TestClass]
	public class SystemTimeUsageAnalyzerTests : CodeFixVerifier
	{
		[TestMethod]
		public void AnalyzeCodeForUsageOfDateTime_WhenThereAreNoDateTimeSimpleMemberAccessExpression_ShouldNotThorwDiagnostic()
		{
			var test = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;

				namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
						}
					}
				}";

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void AnalyzeCodeForUsageOfDateTime_WhenCodeHasDateTimeNowSimpleMemberAccessExpression_ShouldThorwDiagnostic()
		{
			var test = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;

				namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
							var b = DateTime.Now;
						}
					}
				}";

			var expected = new DiagnosticResult
			{
				Id = SystemTimeUsageDiagnosticAnalyzer.DiagnosticId,
				Message = "The use of DateTime.Now is not allowed, use SystemTime instead",
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[]
					{
						new DiagnosticResultLocation("Test0.cs", 16, 16)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void AnalyzeCodeForUsageOfDateTime_WhenCodeHasDateTimeTodaySimpleMemberAccessExpression_ShouldThorwDiagnostic()
		{
			var test = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;

				namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
							var b = DateTime.Today;
						}
					}
				}";

			var expected = new DiagnosticResult
			{
				Id = SystemTimeUsageDiagnosticAnalyzer.DiagnosticId,
				Message = "The use of DateTime.Today is not allowed, use SystemTime instead",
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[]
					{
						new DiagnosticResultLocation("Test0.cs", 16, 16)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void AnalyzeCodeForUsageOfDateTime_WhenCodeHasDateTimeUtcNowSimpleMemberAccessExpression_ShouldThorwDiagnostic()
		{
			var test = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;

				namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
							var b = DateTime.UtcNow;
						}
					}
				}";

			var expected = new DiagnosticResult
			{
				Id = SystemTimeUsageDiagnosticAnalyzer.DiagnosticId,
				Message = "The use of DateTime.UtcNow is not allowed, use SystemTime instead",
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[]
					{
						new DiagnosticResultLocation("Test0.cs", 16, 16)
					}
			};

			VerifyCSharpDiagnostic(test, expected);


		}

		[TestMethod]
		public void AnalyzeCodeForUsageOfDateTime_WhenCodeHasSystemTimeNowSimpleMemberAccessExpression_ShouldNotThorwDiagnostic()
		{
			var test = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;

				namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
							var b = SystemTime.Now;
						}
					}
				}";

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void SystemTimeUsageCodeFixProvider_WhenMissingUsingStatement_ShouldAddUsingStatementAndFixCode()
		{
			var beforeFix = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;

				namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
							var b = DateTime.UtcNow;
						}
					}
				}";

			var afterFix = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;
using OhioBox.Time;

namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
							var b = SystemTime.UtcNow;
						}
					}
				}";


			VerifyCSharpFix(beforeFix, afterFix, allowNewCompilerDiagnostics:true);
		}

		[TestMethod]
		public void SystemTimeUsageCodeFixProvider_WhenCodeHasDateTimeUsage_ShouldReplaceWithSystemTime()
		{
			var beforeFix = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;
				using OhioBox.Time;

				namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
							var b = DateTime.UtcNow;
						}
					}
				}";

			var afterFix = @"
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using System.Text;
				using System.Threading.Tasks;
				using System.Diagnostics;
				using OhioBox.Time;

				namespace ConsoleApplication1
				{
					class TypeName
					{
						public static void Main()
						{
							var a = new DateTime();
							var b = SystemTime.UtcNow;
						}
					}
				}";


			VerifyCSharpFix(beforeFix, afterFix, allowNewCompilerDiagnostics: true);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new SystemTimeUsageDiagnosticAnalyzer();
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new SystemTimeUsageCodeFixProvider();
		}
	}
}