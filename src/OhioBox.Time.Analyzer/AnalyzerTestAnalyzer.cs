using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerTest
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AnalyzerTestAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "DateTimeUsage";
		private const string Title = "DateTime is not allowed";
		private const string Description = "use SystemTime";
		private const string Parameter = "Parameter";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, $"{Parameter} {Title}", "DateTime.Now is not allowed, use SystemTime.now insted'", Parameter, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
		}

		private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
		{
			var expressionSyntax = (MemberAccessExpressionSyntax) context.Node;
			

			var identifier = ((IdentifierNameSyntax)expressionSyntax.Expression).Identifier;
			if (expressionSyntax.Name.Identifier.ValueText == "Now" && expressionSyntax.OperatorToken.ValueText == "." && identifier.ValueText == "DateTime")
			{
				var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), identifier);
				context.ReportDiagnostic(diagnostic);
			}
		}


		private void AnalyzeSyntaxVariableNode(SyntaxNodeAnalysisContext context)
		{
			try
			{
				VariableDeclarationSyntax variable = (VariableDeclarationSyntax)context.Node;
				foreach (var variableDeclaratorSyntax in variable.Variables)
				{
					ReportDateTimePatameters(context, variable.Type, variable.GetLocation(), variableDeclaratorSyntax.Identifier);
				}

				if (variable.Type is IdentifierNameSyntax identifierNameSyntax && identifierNameSyntax.IsVar)
				{
					var a=DateTime.Now;
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private static void AnalyzeSyntaxParameterNode(SyntaxNodeAnalysisContext context)
		{
			try
			{
				ParameterSyntax param = (ParameterSyntax)context.Node;
				ReportDateTimePatameters(context, param.Type, param.GetLocation(), param.Identifier);
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private static void ReportDateTimePatameters(SyntaxNodeAnalysisContext context, TypeSyntax typeSyntax, Location location, SyntaxToken syntaxToken)
		{
			if (typeSyntax is IdentifierNameSyntax identifierNameSyntax)
				IdentifierNameSyntaxDiagnosticReport(context, location, syntaxToken, identifierNameSyntax);

			if (typeSyntax is GenericNameSyntax genericNameSyntax)
			{
				var typeArgumentList = genericNameSyntax.TypeArgumentList.Arguments;
				foreach (var argument in typeArgumentList)
				{
					if (argument is IdentifierNameSyntax argumentIdentifierSyntax)
						IdentifierNameSyntaxDiagnosticReport(context, location, syntaxToken, argumentIdentifierSyntax);
				}
			}
		}

		private static void IdentifierNameSyntaxDiagnosticReport(SyntaxNodeAnalysisContext context, Location location, SyntaxToken syntaxToken, IdentifierNameSyntax identifierNameSyntax)
		{
			var parameterName = identifierNameSyntax.Identifier.Value;
			var type = typeof(DateTime);
			if (parameterName != null && parameterName.ToString() == type.Name)
			{
				var rule = new DiagnosticDescriptor(DiagnosticId, $"{Parameter} {Title}",
					$"{Parameter} '{{0}}' is of type {type.Name} and is not allowed'", Parameter, DiagnosticSeverity.Warning,
					isEnabledByDefault: true);
				var diagnostic = Diagnostic.Create(rule, location, syntaxToken);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
