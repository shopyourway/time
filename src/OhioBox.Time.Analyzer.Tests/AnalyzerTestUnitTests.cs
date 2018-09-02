using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace OhioBox.Time.Analyzer.Tests
{
	[TestClass]
	public class UnitTest : CodeFixVerifier
	{
		[TestMethod]
		public void Test_NoDateTimeSimpleMemberAccessExpression_NoError()
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
		public void Test_WithDateTimeNowSimpleMemberAccessExpression_ThrowError()
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
				Id = DateTimeAnalyzer.DiagnosticId,
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
		public void Test_WithDateTimeTodaySimpleMemberAccessExpression_ThrowError()
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
				Id = DateTimeAnalyzer.DiagnosticId,
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
		public void Test_WithDateTimeUtcNowSimpleMemberAccessExpression_ThrowError()
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
				Id = DateTimeAnalyzer.DiagnosticId,
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

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new DateTimeAnalyzer();
		}
	}
}