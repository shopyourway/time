using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OhioBox.Time.Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DateTimeAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "DateTimeUsage";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, "Access DateTime is not allowed",
			"The use of DateTime.{0} is not allowed, use SystemTime instead", "Current time usage", DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
		}

		private static void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
		{
			var expressionSyntax = (MemberAccessExpressionSyntax)context.Node;

			if (!(expressionSyntax.Expression is IdentifierNameSyntax expression))
				return;

			var identifier = expression.Identifier;
			if (UseDateTime(identifier, expressionSyntax))
			{
				var memberProp = expressionSyntax.Name.Identifier.ValueText;
				var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), memberProp);
				context.ReportDiagnostic(diagnostic);
			}
		}

		private static bool UseDateTime(SyntaxToken simpleMemberIdentifier, MemberAccessExpressionSyntax expressionSyntax)
		{
			var memberName = simpleMemberIdentifier.ValueText;
			var memberOperator = expressionSyntax.OperatorToken.ValueText;
			var memberSelector = expressionSyntax.Name.Identifier.ValueText;
			return memberName == "DateTime" &&
				   memberOperator == "." &&
				   (memberSelector == "Now" || memberSelector == "Today" || memberSelector == "UtcNow");
		}
	}
}